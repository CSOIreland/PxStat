
#Declare DB_DATA
Write-Host "" 
Write-Host "Type the name of the Database to update (eg. pxstat.live):"  



$DbData = Read-host 

#Find current directory path
$scriptDir = $PSScriptRoot 
$delim='"'

#Script filename
$outputScript="CloudScriptUpdate.sql"

#Delete the file if it already exists, otherwise we will end up appending to an existing file
if([System.IO.File]::Exists($scriptDir + '\' + $outputScript))
{
    Remove-Item $scriptDir\$outputScript
}


#Set the database name variable
$setVar=":setvar DB_DATA"
$command=($setVar) + (" ") + $DbData 
Write-Host $command
Add-Content $scriptDir\$outputScript $command 



#Get the version you are trying to update to
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

Write-Host "USE [$DbData]"
Add-Content $scriptDir\$outputScript "USE [$DbData]"


#Drop the stored procedures
$fileName="\Drop\drop_stored_procedures.sql"
Add-Content $scriptDir\$outputScript ":r$delim$scriptDir$fileName$delim"
Write-Host ":r$delim$scriptDir$fileName$delim"

#Drop the views
$fileName="\Drop\drop_views.sql"
Add-Content $scriptDir\$outputScript ":r$delim$scriptDir$fileName$delim"
Write-Host ":r$delim$scriptDir$fileName$delim"

#Drop the types
$fileName="\Drop\drop_types.sql"
Add-Content $scriptDir\$outputScript ":r$delim$scriptDir$fileName$delim"
Write-Host ":r$delim$scriptDir$fileName$delim"

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
                    $fileName=$file.Name
					Add-Content $scriptDir\$outputScript ":r$delim$scriptDir\Scripts\$fileName$delim"
                    Write-Host ":r$delim$scriptDir\Scripts\$fileName$delim"
				}
				catch
				{
					
					Continue
				}
			}
		}


}


#Script the Types
$files = Get-ChildItem -recurse $scriptDir\Types
ForEach($file in $files)
{
    try
    {
        #Directory names do not correspond to script files!
        if($file.GetType().ToString()-eq "System.IO.FileInfo")
        {
            $fname=$file.FullName        
            Write-Host ":r$delim$fname$delim"
            Add-Content $scriptDir\$outputScript ":r$delim$fname$delim"
        }
    }
    catch
    {
        $ErrorMessage = $_.Exception.Message
        Write-Host "ERROR $file - $ErrorMessage"
        "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
        Write-Host ""
        Write-Host "Script Types - Failed"
        Write-Host "********************************************************************************"
        Write-Host ""
        Continue
    }
}

#Script the Views
$files = Get-ChildItem -recurse $scriptDir\Views
ForEach($file in $files)
{
    try
    {
        #Directory names do not correspond to script files!
        if($file.GetType().ToString()-eq "System.IO.FileInfo")
        {
            $fname=$file.FullName
            Write-Host ":r$delim$fname$delim"
            Add-Content $scriptDir\$outputScript ":r$delim$fname$delim"
        }
    }
    catch
    {
        $ErrorMessage = $_.Exception.Message
        Write-Host "ERROR $file - $ErrorMessage"
        "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
        Write-Host ""
        Write-Host "Script Views - Failed"
        Write-Host "********************************************************************************"
        Write-Host ""
        Continue
    }
}

#We need to script the Auditing SPs first -they are used by other SPs

try
{
    #Auditing Create
    $fileName="\StoredProcedures\Security\Security_Auditing_Create.sql"
    Add-Content $scriptDir\$outputScript ":r$delim$scriptDir$fileName$delim"
    Write-Host ":r$scriptDir$delim$fileName$delim"

    #Auditing Delete
    $fileName="\StoredProcedures\Security\Security_Auditing_Delete.sql"
    Add-Content $scriptDir\$outputScript ":r$delim$scriptDir$fileName$delim"
    Write-Host ":r$scriptDir$delim$fileName$delim"

    #Auditing Update
    $fileName="\StoredProcedures\Security\Security_Auditing_Update.sql"
    Add-Content $scriptDir\$outputScript ":r$delim$scriptDir$fileName$delim"
    Write-Host ":r$scriptDir$delim$fileName$delim"
}
catch
    {
        $ErrorMessage = $_.Exception.Message
        Write-Host "ERROR $file - $ErrorMessage"
        "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
        Write-Host ""
        Write-Host "Required Stored Procedures - Failed"
        Write-Host "********************************************************************************"
        Write-Host ""
        Continue
    }

#Script the stored procedures
$files = Get-ChildItem -recurse $scriptDir\StoredProcedures
ForEach($file in $files)
{
try
{
    #Directory names do not correspond to script files!
    if($file.GetType().ToString()-eq "System.IO.FileInfo")
    {
        $fname=$file.FullName
        Write-Host ":r$delim$fname$delim"
        Add-Content $scriptDir\$outputScript ":r$delim$fname$delim"
    }
    }
    catch
    {
        $ErrorMessage = $_.Exception.Message
        Write-Host "ERROR $file - $ErrorMessage"
        "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
        Write-Host ""
        Write-Host "Script Stored Procedures - Failed"
        Write-Host "********************************************************************************"
        Write-Host ""
        Continue
    }
}

#script the Jobs
#Script the stored procedures
$files = Get-ChildItem -recurse $scriptDir\Jobs
ForEach($file in $files)
{
try
{
    #Directory names do not correspond to script files!
    if($file.GetType().ToString()-eq "System.IO.FileInfo")
    {
        $fname=$file.FullName
        Write-Host ":r$delim$fname$delim"
        Add-Content $scriptDir\$outputScript ":r$delim$fname$delim"
    }
    }
    catch
    {
        $ErrorMessage = $_.Exception.Message
        Write-Host "ERROR $file - $ErrorMessage"
        "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
        Write-Host ""
        Write-Host "Script Jobs - Failed"
        Write-Host "********************************************************************************"
        Write-Host ""
        Continue
    }
}

#script the Schedules
#Extract all of the .sql files into an object
$files = Get-ChildItem -recurse $scriptDir\Schedules\*.sql
	
$successCount = 0
$errorCount = 0

#Run each script
ForEach ($file in $files)
{
try
    {
		
		if($file.GetType().ToString()-eq "System.IO.FileInfo")
		{
			$fname=$file.FullName
			Write-Host ":r$delim$fname$delim"
			Add-Content $scriptDir\$outputScript ":r$delim$fname$delim"
		}
    }
    catch
    {
        $ErrorMessage = $_.Exception.Message
        Write-Host "ERROR $file - $ErrorMessage"
        "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
        Write-Host ""
        Write-Host "Script Schedules - Failed"
        Write-Host "********************************************************************************"
        Write-Host ""
        Continue
    }
		
}

Write-Host ""
Write-Host "Your SQL Script is now ready at $($scriptDir)$('\')$($outputScript)"
Write-Host "Please remember to run the script in SQL Command Mode if running from Sql Server Management Studio"
Write-Host "Press any key to finish"
Read-host 