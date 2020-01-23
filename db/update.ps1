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

Write-Host ""
Write-Host "Please wait..."
Write-Host ""
	
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
					"" | out-file $scriptDir\update.log -Append
					"ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
					$errorCount = $errorCount + 1
					Continue
				}
			}
		}

	Write-Host ""
	Write-Host "Script - Errors: $errorCount"
	Write-Host "Script - Success: $successCount"
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
            "" | out-file $scriptDir\update.log -Append
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
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
            "" | out-file $scriptDir\update.log -Append
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
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
            "" | out-file $scriptDir\update.log -Append
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
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
            "" | out-file $scriptDir\update.log -Append
            "ERROR $date : $file - $ErrorMessage" | out-file $scriptDir\update.log -Append
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