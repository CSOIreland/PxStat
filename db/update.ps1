#SQL Objects deployment

cls

#Find current date
$date = get-date -format g


#Find current directory path
$scriptDir = $PSScriptRoot 

# ********************************************************************************
# Connection string
# ********************************************************************************

#Declare Server
Write-Host "" 
Write-Host "Name/IP of the SQL Server Instance:" 
$server = Read-host

#Declare Username
Write-Host "" 
Write-Host "SysAdmin Username:"  
$username = Read-host

#Declare Password
Write-Host "" 
Write-Host "SysAdmin Password:"  
$securePassword = Read-host -AsSecureString
$password =[Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)) 


# ********************************************************************************
# Initiate log
# ********************************************************************************

"********************************************************************************" | out-file $scriptDir\update.log -Append

# ********************************************************************************
# Create Database
# ********************************************************************************

#Declare DB_DATA
Write-Host "" 
Write-Host "Type the name of the Database to update (eg. pxstat.live):"  

$DbData = Read-host 

#Declare DB_VERSION
Write-Host "" 
Write-Host "Type the Version number (eg. 1.1.0) you are updating from:"  
Write-Host "(You can find this information at the bottom-right of the Application's footer):"  

$DbVersion = Read-host 

#Get the ne DB version for the script with the latest version in the Scripts directory
$DbNewVersion = ((Get-ChildItem $scriptDir\Scripts | sort-object name)[-1]).Basename

if([System.Version]$DbVersion -lt "5.0.0" -and [System.Version]$DbNewVersion -gt "5.0.0") {
    Write-Host ""
    Write-Host "This update is from a version less than 5.0.0 i.e. $DbVersion, so you will need to migrate to 5.0.0 first and then run the update again(Please see Migration section at https://https://github.com/CSOIreland/PxStat/wiki/Update-Database)."
    Write-Host ""
    exit
}

Write-Host ""
Write-Host "Please wait..."
Write-Host ""



# ********************************************************************************
# Drop Stored Procedures
# ********************************************************************************

#Extract the file into an object
$files = Get-ChildItem -recurse $scriptDir\Drop\drop_stored_procedures.sql
  
ForEach ($file in $files)
    {
        try
        {
            Invoke-SQLCMD -Username $username -Password $password -Inputfile $file.FullName -serverinstance $server -database $DbData -ErrorAction Stop
            "SUCCESS $date : $file" | out-file $scriptDir\update.log -Append
            Write-Host ""
            Write-Host "Drop Stored Procedures - Success"
            Write-Host "********************************************************************************"
            Write-Host ""
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
            Write-Host ""
            Write-Host "Drop Stored Procedures - Fail"
            Write-Host "********************************************************************************"
            Write-Host ""
            Continue
        }
    }


# ********************************************************************************
# Drop Views
# ********************************************************************************

#Extract the file into an object
$files = Get-ChildItem -recurse $scriptDir\Drop\drop_views.sql

ForEach ($file in $files)
    {
        try
        {
            Invoke-SQLCMD -Username $username -Password $password -Inputfile $file.FullName -serverinstance $server -database $DbData -ErrorAction Stop
            "SUCCESS $date : $file" | out-file $scriptDir\update.log -Append
            Write-Host ""
            Write-Host "Drop Views - Success"
            Write-Host "********************************************************************************"
            Write-Host ""
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
            Write-Host ""
            Write-Host "Drop Views - Fail"
            Write-Host "********************************************************************************"
            Write-Host ""
            Continue
        }
    }

# ********************************************************************************
# Drop Types
# ********************************************************************************

#Extract the file into an object
$files = Get-ChildItem -recurse $scriptDir\Drop\drop_types.sql

ForEach ($file in $files)
    {
        try
        {
            Invoke-SQLCMD -Username $username -Password $password -Inputfile $file.FullName -serverinstance $server -database $DbData -ErrorAction Stop
            "SUCCESS $date : $file" | out-file $scriptDir\update.log -Append
            Write-Host ""
            Write-Host "Drop Types - Success"
            Write-Host "********************************************************************************"
            Write-Host ""
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
            Write-Host ""
            Write-Host "Drop Types - Fail"
            Write-Host "********************************************************************************"
            Write-Host ""
            Continue
        }
    }

# ********************************************************************************
# Run the Script
# ********************************************************************************
if($DbVersion.length -gt 0) {
	#Extract the databse.sql file into an object
	$files = Get-ChildItem -recurse $scriptDir\Scripts\*.sql | Sort-Object

	$successCount = 0
	$errorCount = 0

	ForEach ($file in $files)
		{
        if($file.Name -gt "$DbVersion.sql") 
            {
				try
				{
                    Write-Host "Running: $($file.Name)"
					
					Invoke-SQLCMD -Username $username -Password $password  -Inputfile $file.FullName -serverinstance $server -database $DbData -ErrorAction Stop
					"SUCCESS $date : $file" | out-file $scriptDir\update.log -Append
					$successCount = $successCount + 1
					
				}
				catch
				{
					$ErrorMessage = $_.Exception.Message
					Write-Host "ERROR $file - $ErrorMessage"
					"ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
					$errorCount = $errorCount + 1
					Continue
				}
			}
		}

	Write-Host ""
	Write-Host "Run Scripts - Errors: $errorCount"
	Write-Host "Run Scripts - Success: $successCount"
	Write-Host "********************************************************************************"
	Write-Host ""
}

# ********************************************************************************
# Create or Alter Types
# ********************************************************************************

#Extract all of the .sql files into an object
$files = Get-ChildItem -recurse $scriptDir\Types\*.sql
	
$successCount = 0
$errorCount = 0

#Run each script
ForEach ($file in $files)
    {
        try
        {
            Invoke-SQLCMD -Username $username -Password $password -Inputfile $file.FullName -serverinstance $server -database $DbData -ErrorAction Stop
            "SUCCESS $date : $file" | out-file $scriptDir\update.log -Append
            $successCount = $successCount + 1
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
            $errorCount = $errorCount + 1
            Continue
        }
    }
Write-Host ""
Write-Host "Create Types - Errors: $errorCount"
Write-Host "Create Types - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

# ********************************************************************************
# Create or Alter Views
# ********************************************************************************

#Extract all of the .sql files into an object
$files = Get-ChildItem -recurse $scriptDir\Views\*.sql
	
$successCount = 0
$errorCount = 0


#Run each script
ForEach ($file in $files)
    {
        try
        {
            Invoke-SQLCMD -Username $username -Password $password  -Inputfile $file.FullName -serverinstance $server -database $DbData -ErrorAction Stop
            "SUCCESS $date : $file" | out-file $scriptDir\update.log -Append
            $successCount = $successCount + 1
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
            $errorCount = $errorCount + 1
            Continue
        }
    }
Write-Host ""
Write-Host "Create Views - Errors: $errorCount"
Write-Host "Create Views - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

# ********************************************************************************
# Create or Alter Stored Procedures
# ********************************************************************************

#Extract all of the .sql files into an object
$files = Get-ChildItem -recurse $scriptDir\StoredProcedures\*.sql
	
$successCount = 0
$errorCount = 0


#Run each script
ForEach ($file in $files)
    {
        try
        {
            Invoke-SQLCMD -Username $username -Password $password  -Inputfile $file.FullName -serverinstance $server -database $DbData -ErrorAction Stop
            "SUCCESS $date : $file" | out-file $scriptDir\update.log -Append
            $successCount = $successCount + 1
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
            $errorCount = $errorCount + 1
            Continue
        }
    }
Write-Host ""
Write-Host "Create Stored Procedures - Errors: $errorCount"
Write-Host "Create Stored Procedures - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

# ********************************************************************************
# Create or Alter Jobs
# ********************************************************************************

#Extract all of the .sql files into an object
$files = Get-ChildItem -recurse $scriptDir\Jobs\*.sql
	
$successCount = 0
$errorCount = 0

#Run each script
ForEach ($file in $files)
    {
        try
        {
            Invoke-SQLCMD -Username $username -Password $password  -Inputfile $file.FullName -Variable @("DB_DATA=$DbData") -serverinstance $server -ErrorAction Stop
            "SUCCESS $date : $file" | out-file $scriptDir\update.log -Append
            $successCount = $successCount + 1
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
            $errorCount = $errorCount + 1
            Continue
        }
    }
Write-Host ""
Write-Host "Create Jobs - Errors: $errorCount"
Write-Host "Create Jobs - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

# ********************************************************************************
# Create or Alter Schedules
# ********************************************************************************

#Extract all of the .sql files into an object
$files = Get-ChildItem -recurse $scriptDir\Schedules\*.sql
	
$successCount = 0
$errorCount = 0

#Run each script
ForEach ($file in $files)
    {
        try
        {
            Invoke-SQLCMD -Username $username -Password $password  -Inputfile $file.FullName -Variable @("DB_DATA=$DbData") -serverinstance $server -ErrorAction Stop
            "SUCCESS $date : $file" | out-file $scriptDir\update.log -Append
            $successCount = $successCount + 1
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
            $errorCount = $errorCount + 1
            Continue
        }
    }
Write-Host ""
Write-Host "Create Schedules - Errors: $errorCount"
Write-Host "Create Schedules - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

Write-Host ""
Write-Host "Script completed"