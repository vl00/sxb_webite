./publish.ps1 debug
$serverHost = "10.1.0.16";
$userName = "Sxkid.com\James";
$password = ConvertTo-SecureString "Yjx@9394" -AsPlainText -Force
$cred = New-Object System.Management.Automation.PSCredential($userName,$password)
$Session = New-PSSession -ComputerName $serverHost -Credential $cred
Copy-Item ./publish.zip -Destination "D:\Projects\iSchool.V4\iSchool.Inside\publish.zip" -ToSession $Session 
Remove-Item ./publish.zip -Force
Invoke-Command -Session $Session { 
cd D:\Projects\iSchool.V4\iSchool.Inside\
.\publish.ps1
echo 1
}