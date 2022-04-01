
#Declare DB_DATA
Write-Host "" 
Write-Host "Type the name of the Database to install (eg. pxstat.live):"  



$DbData = Read-host 

#Find current directory path
$scriptDir = $PSScriptRoot 
$delim='"'

#Script filename
$outputScript="CloudScriptInstall.sql"

#Delete the file if it already exists, otherwise we will end up appending to an existing file
if([System.IO.File]::Exists($outputScript))
{
    Remove-Item $outputScript
}


#Set the database name variable
$setVar=":setvar DB_DATA"
$command=($setVar) + (" ") + $DbData 
Write-Host $command
Add-Content $scriptDir\$outputScript $command 




#Declare Application Administrator


DO {
Write-Host "" 
Write-Host "Do you want to use an Active Directory or Local account to access the application for the first time?" 
Write-Host "(Type 'ad' for Active Directory or 'local' for Local)"   

	$accountType = Read-host 
}
while ("ad", "local" -notcontains $accountType)

switch ( $accountType )
    {
        "ad" 
            { 
            $ccnAdFlag = 1

            Write-Host "" 
            Write-Host "Please provide the 'SamAccountName' (Security Account Manager) of the Active Directory account".
            Write-Host "N.B. This account will be able to access the application with Single Sign-On and Administrator rights."

            $ccnUsername = Read-host 
            $ccnEmail = "*********" #SQL placeholder for NULL
            $ccnDisplayName = "*********" #SQL placeholder for NULL
            }
        "local" 
            { 
            $ccnAdFlag = 0
            
            Write-Host "" 
            Write-Host "Please provide the email address for the Local account."
            Write-Host "The Password can be set using the 'Forgotten Password' link available in the Login interface."
            Write-Host "The 2FA (Two-Factor Authentication) can be set using the 'Reset 2FA' link in the Login interface."

            $ccnEmail = Read-host 
            $ccnUsername = $ccnEmail

            Write-Host "" 
            Write-Host "Please provide the full name (eg. John Murphy) for the Local account".
            
            $ccnDisplayName = Read-host
            }
    }
#Set CCN_AD_FLAG variable
$setVar=":setvar CCN_AD_FLAG"
$command=($setVar) + (" ") + $ccnAdFlag 
Write-Host $command
Add-Content $scriptDir\$outputScript $command 

#Set CCN_USERNAME variable
$setVar=":setvar CCN_USERNAME"
$command=($setVar) + (" ") + $ccnUsername 
Write-Host $command
Add-Content $scriptDir\$outputScript $command 

#Set CCN_EMAIL variable
$setVar=":setvar CCN_EMAIL"
$command=($setVar) + (" ") + $ccnEmail 
Write-Host $command
Add-Content $scriptDir\$outputScript $command 

#Set CCN_DISPLAYNAME variable
$setVar=":setvar CCN_DISPLAYNAME"
$command=($setVar) + (" ") + ($delim) + $ccnDisplayName + ($delim)
Write-Host $command
Add-Content $scriptDir\$outputScript $command 


#Create and setup the database  
$fullFileName=($scriptDir) + "\databaseCloud.sql"
Write-Host ":r$delim$fullFileName$delim"
Add-Content $scriptDir\$outputScript ":r$delim$fullFileName$delim"

#Script the main database entities
$fullFileName=($scriptDir) + "\datamodel.sql"
Write-Host ":r$delim$fullFileName$delim"
Add-Content $scriptDir\$outputScript ":r$delim$fullFileName$delim"


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
        Write-Host "Pre requisite Stored Procedures - Failed"
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


# ********************************************************************************
# Create Schedules
# ********************************************************************************

#Extract all of the .sql files into an object
$files = Get-ChildItem -recurse $scriptDir\Schedules\*.sql
	
$successCount = 0
$errorCount = 0

#Run each script
ForEach ($file in $files)
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
Write-Host "Schedules - Errors: $errorCount"
Write-Host "Schedules - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

Write-Host ""
Write-Host "Your SQL Script is now ready at $($scriptDir)$('\')$($outputScript)"
Write-Host "Please remember to run the script in SQL Command Mode if running from Sql Server Management Studio"
Write-Host "Press any key to finish"
Read-host 