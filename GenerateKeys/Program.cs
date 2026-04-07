using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

if (args.Length == 0)
{
    Console.WriteLine("First parameter must be the certificate destination path.");
    return;
}

string path = args[0];

string caCertificatePath = Path.Join(path, "QuickFixn-TestCA.cer");
string serverPfxCertificatePath = Path.Join(path, "QuickFixn-TestServer.pfx");
string clientPfxCertificatePath = Path.Join(path, "QuickFixn-TestClient.pfx");

const string pfxPassword = "qfnpass123";

var caCertificate = CreateCaCertificate();
Console.WriteLine($"Writing CACertificate: {caCertificatePath}");
File.WriteAllBytes(caCertificatePath, caCertificate.Export(X509ContentType.Cert));

var serverCertificate = CreateServerCertificate(caCertificate);
Console.WriteLine($"Writing ServerCertificate: {serverPfxCertificatePath}");
File.WriteAllBytes(serverPfxCertificatePath, serverCertificate.Export(X509ContentType.Pfx, pfxPassword));

var clientCertificate = CreateClientCertificate(caCertificate);
Console.WriteLine($"Writing ClientCertificate: {clientPfxCertificatePath}");
File.WriteAllBytes(clientPfxCertificatePath, clientCertificate.Export(X509ContentType.Pfx, pfxPassword));

return;


static X509Certificate2 CreateCaCertificate()
{
    using var rsa = RSA.Create();
    var request = new CertificateRequest("CN=QuickFixn-TestCA", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
    request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.DigitalSignature, true));

    X509Certificate2 certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30));
    return certificate;
}

static X509Certificate2 CreateServerCertificate(X509Certificate2 caCertificate)
{
    using var rsa = RSA.Create();
    using (RSA? caPrivateKey = caCertificate.GetRSAPrivateKey())
    {
        var request = new CertificateRequest($"CN=QuickFixn-TestServer", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));

        var sanBuilder = new SubjectAlternativeNameBuilder();
        sanBuilder.AddDnsName("localhost");
        sanBuilder.AddIpAddress(IPAddress.Loopback);
        request.CertificateExtensions.Add(sanBuilder.Build());

        var enhancedKeyUsages = new OidCollection
        {
            new Oid("1.3.6.1.5.5.7.3.1"), // OID for Server Authentication
        };
        request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(enhancedKeyUsages, true));

        X509Certificate2 certificate = request.Create(caCertificate, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(5), [0, 0, 0, 0, 0, 0, 0, 1]);
        return certificate.CopyWithPrivateKey(rsa);
    }
}

static X509Certificate2 CreateClientCertificate(X509Certificate2 caCertificate)
{
    using var rsa = RSA.Create();
    using (RSA? caPrivateKey = caCertificate.GetRSAPrivateKey())
    {
        var request = new CertificateRequest($"CN=QuickFixn-TestClient", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));

        var sanBuilder = new SubjectAlternativeNameBuilder();
        sanBuilder.AddDnsName("localhost");
        sanBuilder.AddIpAddress(IPAddress.Loopback);
        request.CertificateExtensions.Add(sanBuilder.Build());

        var enhancedKeyUsages = new OidCollection
        {
            new Oid("1.3.6.1.5.5.7.3.2")  // OID for Client Authentication
        };
        request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(enhancedKeyUsages, true));

        X509Certificate2 certificate = request.Create(caCertificate, DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(5), [0, 0, 0, 0, 0, 0, 0, 2]);
        return certificate.CopyWithPrivateKey(rsa);
    }
}
