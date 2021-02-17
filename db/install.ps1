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

"********************************************************************************" | out-file $scriptDir\install.log -Append

# ********************************************************************************
# Create Database
# ********************************************************************************

#Declare DB_DATA
Write-Host "" 
Write-Host "Choose the name of the new Database (eg. PxStat):"  

$DbData = Read-host 

DO {
	#Declare DB_RECOVERY
	Write-Host "" 
	Write-Host "Choose the Database recovery model between SIMPLE (recommended), BULK_LOGGED, FULL"  
	Write-Host "See Microsoft Documentation at https://docs.microsoft.com/en-us/sql/relational-databases/backup-restore/recovery-models-sql-server"  

	$DbRecovery = Read-host 
}
while ("FULL", "SIMPLE", "BULK_LOGGED" -notcontains $DbRecovery)

#Declare DB_DATA_PATH
Write-Host "" 
Write-Host "Choose the folder where to store the Database file (.mdf):"  
Write-Host "(eg. E:\sqldata\data)"  

$DbDataPath = Read-host 
if ([string]::IsNullOrWhiteSpace($DbDataPath)) { $DbDataPath  = 'E:\sqldata\data' }

#Declare DB_LOG
$DbLog = "$($DbData)_log"

#Declare DB_LOG_PATH
Write-Host "" 
Write-Host "Choose the folder where to store the Log file (.ldf):"  
Write-Host "(eg. E:\sqldata\log)"  

$DbLogPath = Read-host 
if ([string]::IsNullOrWhiteSpace($DbLogPath)) { $DbLogPath  = 'E:\sqldata\log' }

#Extract the databse.sql file into an object
$files = Get-ChildItem -recurse $scriptDir\database.sql

$successCount = 0
$errorCount = 0
  
Write-Host ""
Write-Host "Please wait..."
Write-Host ""

ForEach ($file in $files)
    {
        try
        {
            Invoke-SQLCMD -Username $username -Password $password -Inputfile $file.FullName -Variable $("DB_DATA=$DbData", "DB_RECOVERY=$DbRecovery", "DB_DATA_PATH=$DbDataPath", "DB_LOG=$DbLog", "DB_LOG_PATH=$DbLogPath") -serverinstance $server -ErrorAction Stop
            "SUCCESS $date : $file" | out-file $scriptDir\install.log -Append
            $successCount = $successCount + 1
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\install.log -Append
            $errorCount = $errorCount + 1
            Continue
        }
    }

Write-Host ""
Write-Host "Database - Errors: $errorCount"
Write-Host "Database - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

# ********************************************************************************
# Create PxStat Login
# ********************************************************************************

#Declare PxStat Login
DO {
	Write-Host "" 
	Write-Host "Do you want to create the 'pxstat' SQL Server Login?" 
	Write-Host "This will create the SQL Server Login with a random Password, used by the application in the Database Connection String." 
	Write-Host "Please change the Password in SSMS (SQL Server Management Studio) and update the Database Connection String in the Web.config of the Application."
	Write-Host "N.B. This is required ONLY for the first time this script runs." 
	Write-Host "(Press 'y' for yes or 'n' for no)"   

	$addLoginConfirm = Read-host 
}
while ("y", "n" -notcontains $addLoginConfirm)

Write-Host ""
Write-Host "Please wait..."
Write-Host ""

#Add PxStat Login
if ("y" -contains $addLoginConfirm)
   {
    $successCount = 0
    $errorCount = 0
	
    #Extract the user.sql file into an object
    $files = Get-ChildItem -recurse $scriptDir\login.sql
    
	ForEach ($file in $files)
		{
			try
			{
				Invoke-SQLCMD -Username $username -Password $password -Inputfile $file.FullName -serverinstance $server -ErrorAction Stop
				"SUCCESS $date : $file" | out-file $scriptDir\install.log -Append
				$successCount = $successCount + 1
			}
			catch
			{
				$ErrorMessage = $_.Exception.Message
				Write-Host "ERROR $file - $ErrorMessage"
				"ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\install.log -Append
				$errorCount = $errorCount + 1
				Continue
			}
		}

	Write-Host ""
	Write-Host "Login - Errors: $errorCount"
	Write-Host "Login - Success: $successCount"
	Write-Host "********************************************************************************"
	Write-Host ""

    $successCount = 0
    $errorCount = 0

    #Extract the msdb.sql file into an object
    $files = Get-ChildItem -recurse $scriptDir\msdb.sql
    
	ForEach ($file in $files)
		{
			try
			{
				Invoke-SQLCMD -Username $username -Password $password -Inputfile $file.FullName -serverinstance $server -ErrorAction Stop
				"SUCCESS $date : $file" | out-file $scriptDir\install.log -Append
				$successCount = $successCount + 1
			}
			catch
			{
				$ErrorMessage = $_.Exception.Message
				Write-Host "ERROR $file - $ErrorMessage"
				"ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\install.log -Append
				$errorCount = $errorCount + 1
				Continue
			}
		}

	Write-Host ""
	Write-Host "msdb - Errors: $errorCount"
	Write-Host "msdb - Success: $successCount"
	Write-Host "********************************************************************************"
	Write-Host ""
}

# ********************************************************************************
# Create Datamodel
# ********************************************************************************

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

#Extract the datamodel.sql file into an object
$files = Get-ChildItem -recurse $scriptDir\datamodel.sql

$successCount = 0
$errorCount = 0
  
Write-Host ""
Write-Host "Please wait..."
Write-Host ""

ForEach ($file in $files)
    {
        try
        {
            Invoke-SQLCMD -Username $username -Password $password -Inputfile $file.FullName -Variable @("CCN_USERNAME=$ccnUsername", "CCN_DISPLAYNAME=$ccnDisplayName", "CCN_EMAIL=$ccnEmail", "CCN_AD_FLAG=$ccnAdFlag") -serverinstance $server -database $DbData -ErrorAction Stop
            "SUCCESS $date : $file" | out-file $scriptDir\install.log -Append
            $successCount = $successCount + 1
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\install.log -Append
            $errorCount = $errorCount + 1
            Continue
        }
    }

Write-Host ""
Write-Host "Datamodel - Errors: $errorCount"
Write-Host "Datamodel - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

# ********************************************************************************
# Create Types
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
            "SUCCESS $date : $file" | out-file $scriptDir\install.log -Append
            $successCount = $successCount + 1
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\install.log -Append
            $errorCount = $errorCount + 1
            Continue
        }
    }
Write-Host ""
Write-Host "Types - Errors: $errorCount"
Write-Host "Types - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

# ********************************************************************************
# Create Views
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
            "SUCCESS $date : $file" | out-file $scriptDir\install.log -Append
            $successCount = $successCount + 1
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\install.log -Append
            $errorCount = $errorCount + 1
            Continue
        }
    }
Write-Host ""
Write-Host "Views - Errors: $errorCount"
Write-Host "Views - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

# ********************************************************************************
# Create Stored Procedures
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
            "SUCCESS $date : $file" | out-file $scriptDir\install.log -Append
            $successCount = $successCount + 1
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\install.log -Append
            $errorCount = $errorCount + 1
            Continue
        }
    }
Write-Host ""
Write-Host "Stored Procedures - Errors: $errorCount"
Write-Host "Stored Procedures - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

# ********************************************************************************
# Create Jobs
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
            "SUCCESS $date : $file" | out-file $scriptDir\install.log -Append
            $successCount = $successCount + 1
        }
        catch
        {
            $ErrorMessage = $_.Exception.Message
            Write-Host "ERROR $file - $ErrorMessage"
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\install.log -Append
            $errorCount = $errorCount + 1
            Continue
        }
    }
Write-Host ""
Write-Host "Jobs - Errors: $errorCount"
Write-Host "Jobs - Success: $successCount"
Write-Host "********************************************************************************"
Write-Host ""

Write-Host ""
Write-Host "Script completed"