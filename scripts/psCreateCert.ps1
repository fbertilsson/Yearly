#
# Creates a new self signed certificate for TLS and exports it
# - Must run as admin (access to cert store)
#
# N.B. This does not yet work. It produces a cert for CLIENT authentication:
#         Key Usage = Digital Signature, Key Encipherment (a0)
#      when it should (probably) produce:
#         Key Usage = Digital Signature, Key Encipherment (e0), Non-Repudiation

$dnsName = 'fbmetering.azurewebsites.net'
$cert = New-SelfSignedCertificate -DnsName $dnsName -CertStoreLocation cert:\LocalMachine\My -HashAlgorithm sha256 -NotAfter 2020-02-02
Write-Host $cert
$password = ConvertTo-SecureString -String "everyYearJames" -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath ".\$dnsName.pfx" -Password $password
