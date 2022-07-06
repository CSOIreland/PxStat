using API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Px5Migrator
{
    public class Px5Migrator
    {
        private static int migrateCount = 0;
        private static bool migrate;
        private static bool validate;
        private static bool truncate;


        static void Main(string[] args)
        {
            if (args == null)
            {
                nullArgs();
                return;
            }
            if (args.Contains("-m") || args.Contains("-M")) migrate = true; else migrate = false;
            if (args.Contains("-v") || args.Contains("-V")) validate = true; else validate = false;
            if (args.Contains("-t") || args.Contains("-T")) truncate = true; else truncate = false;

            if (args.Length == 0)
            {
                nullArgs();
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Supervisor supy = new Supervisor();
            var result = supy.GetReport();
            Console.WriteLine($"{result.ItemsRemainingCount} matrixes to be migrated");

            StartMigration();


            string tString = DateTime.Now.ToString("HH:mm:ss");
            Log.Instance.Debug($"Migration started at {tString}");
            sw.Stop();
            int secs = (int)(sw.ElapsedMilliseconds / 1000);
            tString = DateTime.Now.ToString("HH:mm:ss");
            Log.Instance.Debug($"Migration ended at {tString}");
            Log.Instance.Debug($"Migration script complete in {secs} seconds");
            Console.WriteLine($"Migration script complete in {secs} seconds");
            Console.WriteLine($"{migrateCount} matrixes have been migrated in this session");


            result = supy.GetReport();
            Console.WriteLine($"{result.ItemsMigratedCount} matrixes in the database are marked as migrated");
            Console.WriteLine($"{result.ItemsRemainingCount} matrixes in the database are yet to be migrated");
            Console.WriteLine($"Press any key to finish");
            Console.ReadLine();
        }

        private static void nullArgs()
        {
            Console.WriteLine("Use the following options for the Px5Migrator.exe: ");
            Console.WriteLine("-m     Migrate all unmigrated matrixes");
            Console.WriteLine("-v     Read back and validate each matrix after it has been migrated");
            Console.WriteLine("-t     Truncate the tables associated with the old data structure once migration has completed");
            Console.WriteLine(" a space separated list of integers: only migrate the list of matrix ids in the list");
        }

        private static void StartMigration()
        {


            List<MigrateCandidate> allCandidates = new List<MigrateCandidate>();
            using (IADO ado = IAdoFactory.GetAdo())
            {
                Migration_ADO mAdo = new Migration_ADO();
                var result = mAdo.ReadUnmigratedList(ado);
                if (result.hasData)
                {
                    foreach (var item in result.data)
                    {
                        MigrateCandidate cnd = new MigrateCandidate() { MtrCode = item.MtrCode, MtrId = item.MtrId };
                        allCandidates.Add(cnd);

                    }
                }
            }

            if (migrate || allCandidates.Count > 0)
            {

                foreach (var mtr in allCandidates)
                {
                    DoMigration(true, mtr.MtrId);
                }

                Supervisor supy = new Supervisor();
                var tresult = supy.GetReport();
                Console.WriteLine($"{tresult.ItemsMigratedCount} matrixes in the database are marked as migrated");
                Console.WriteLine($"{tresult.ItemsRemainingCount} matrixes in the database are yet to be migrated");
            }
            else
            {
                Supervisor supy = new Supervisor();
                var result = supy.GetReport();
                result = supy.GetReport();
                Console.WriteLine($"{result.ItemsMigratedCount} matrixes in the database are marked as migrated");
                Console.WriteLine($"{result.ItemsRemainingCount} matrixes in the database are yet to be migrated");
                nullArgs();
            }

            if (truncate)
            {
                Console.WriteLine("Deleting old structure data...");
                using (IADO ado = IAdoFactory.GetAdo())
                {
                    Migration_ADO mAdo = new Migration_ADO();
                    int deleteCount = mAdo.DeleteOldStructureData(ado);
                    Console.WriteLine($"Old structure data removed for {deleteCount} datasets.");
                }
            }


        }


        private static SupervisoryReport DoSupyReport()
        {
            return new SupervisoryReport();
        }




        private static bool DoMigration(bool readAndValidate = true, int mtrId = 0)
        {
            bool complete = true;
            try
            {
                MigrationReport report = null;
                int matrixId = 0;
                string lngIsoCode = null;
                string frqCode = "";
                string frqValue = "";


                //Get the next unmigrated matrix
                Migration_ADO mAdo = new Migration_ADO();
                string pxInput = "";

                ADO_readerOutput response = null;


                lngIsoCode = null;
                using (IADO ado = IAdoFactory.GetAdo())
                {
                    try
                    {
                        response = mAdo.ReadNextUnmigrated(ado, mtrId);


                        if (response.hasData)
                        {
                            pxInput = response.data[0].MtrInput;
                            matrixId = response.data[0].MtrId;
                            lngIsoCode = response.data[0].LngIsoCode;
                            complete = true;
                        }
                        else
                        {
                            report = new MigrationReport();
                            report.Errors = new List<Exception>
                        {
                            new Exception("No unmigrated data found")
                        };
                            return false;
                        }

                        response = mAdo.GetFrequencyDetails(ado, matrixId);
                        if (response.hasData)
                        {
                            frqCode = response.data[0].FrqCode;
                            frqValue = response.data[0].FrqValue;
                        }
                        else
                        {
                            report = new MigrationReport();
                            report.Errors = new List<Exception>
                        {
                            new Exception("Frequency data not found")
                        };
                            Console.WriteLine($"Ending migration for matrix id {matrixId}...");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error($"Error reading data {ex.Message}");

                    }
                }

                IDocument pxdoc = PxStatEngine.CreatePxDocument(pxInput);


                IUpload_DTO dto = new PxUpload_DTO()
                {
                    MtrInput = pxInput,
                    LngIsoCode = lngIsoCode,
                    FrqCodeTimeval = frqCode,
                    FrqValueTimeval = frqValue

                };
                dto.LngIsoCode = lngIsoCode;
                IDmatrix matrix = null;
                try
                {
                    matrix = new Dmatrix(pxdoc, dto, null, new MetaData());
                    matrix.Id = matrixId;
                    matrix.Dspecs[lngIsoCode].MatrixId = matrixId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create matrix for matrix id {matrixId}. Error: {ex.Message}");
                    Log.Instance.Error($"Failed to create matrix for matrix id {matrixId}. Error: {ex.Message}");
                    return false;
                }

                //for validation
                matrix.Language = lngIsoCode;
                matrix.Languages = new List<string>();
                matrix.Languages.Add(lngIsoCode);

                DMatrix_VLD validator = new DMatrix_VLD();
                var vresults = validator.Validate(matrix);
                if (!vresults.IsValid)
                {
                    report = new MigrationReport();
                    report.MatrixId = matrix.Id;
                    report.MtrCode = matrix.Code;
                    report.Errors = new List<Exception>();

                    foreach (var error in vresults.Errors)
                    {
                        report.Errors.Add(new Exception(error.ErrorMessage));
                    }
                    Console.WriteLine($"Validation error for {matrix.Code}, ending..");
                    foreach (var err in report.Errors)
                    {
                        Console.WriteLine(err.Message);
                    }
                    Log.Instance.Debug($"Invalid matrix found: {matrix.Code}. This matrix was not migrated");
                    Console.WriteLine($"Invalid matrix found: {matrix.Code}. This matrix was not migrated");
                    return false;

                }

                DataWriter dw = new DataWriter();

                using (IADO ado = IAdoFactory.GetAdo())
                {
                    try
                    {
                        ado.StartTransaction();
                        dto.LngIsoCode = lngIsoCode;
                        dw.CreateAndLoadDataField(ado, matrix, dto.LngIsoCode);
                        dw.CreateAndLoadMetadata(ado, matrix, dto.LngIsoCode);
                        dw.UpdateMatrixStatus(ado, matrixId, true);
                        migrateCount++;
                        ado.CommitTransaction();
                        string tString = DateTime.Now.ToString("HH:mm:ss");
                        Log.Instance.Debug($"Migration complete for {matrix.Code},  matrix id {matrixId}, at time {tString}, total={migrateCount}");
                        Console.WriteLine($"Migration complete for {matrix.Code},  matrix id {matrixId}, at time {tString}");
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error($"Error migrating matrix {matrix.Code}, message: {ex.Message}");
                        return false;
                    }

                }

                if (readAndValidate)
                {
                    try
                    {
                        DataReader dr = new DataReader();
                        using (IADO ado = new ADO("defaultConnection"))
                        {
                            var mtr = dr.ReadReleaseDmatrix(ado, matrix.Id);
                            DMatrix_VLD vld = new DMatrix_VLD();
                            var vresult = vld.Validate(mtr);
                            if (vresult.IsValid)
                            {
                                Console.WriteLine($"Validation ok for matrix {matrix.Code} matrix id {matrix.Id} ");
                                Log.Instance.Debug($"Validation ok for matrix {matrix.Code} matrix id {matrix.Id} ");
                            }
                            else
                            {
                                Console.WriteLine($"VALIDATION FAILED for matrix {matrix.Code} matrix id {matrix.Id} ");
                                Log.Instance.Debug($"VALIDATION FAILED for matrix {matrix.Code} matrix id {matrix.Id} ");

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error($"Error validating matrix {matrix.Code}, error: {ex.Message}");

                    }
                }


            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex.Message);
                return false;
            }


            return complete;
        }


    }

    public class MigrateCandidate
    {
        public int MtrId { get; set; }
        public string MtrCode { get; set; }

    }
}
