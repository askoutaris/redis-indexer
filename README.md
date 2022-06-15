# type-signature
Creates the signature of the entire structure of a type (including all its properties, fields and children)

### TypeSignature Usage
```csharp
ITypeScanner typeScanner = new TypeScanner();
IHashGenerator hashGenerator = new SHA512HashGenerator();
ISignatureBuilder signatureBuilder = new SignatureBuilder(typeScanner, hashGenerator);

string signature = signatureBuilder.GetSignature<Person>();

Console.WriteLine($"The signature of type Person is: {signature}");

//The signature of type Person is: b1e38d81d7b812739b0fd09e053a1ecf1936144619e4452ac8633feea6ad41fe
```

### ASP.NET Core

In order to use TypeSignature with ASP.Net Core you have to install <a href="https://www.nuget.org/packages/TypeSignature.Extensions.DependencyInjection/" target="_blank">TypeSignature.Extensions.DependencyInjection</a> nuget package

```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddTypeSignatureSHA256();
  // or
  services.AddTypeSignatureSHA512();
}
```

### Microsoft Dependency Injection

In order to use TypeSignature with Microsoft Dependency Injection you have to install <a href="https://www.nuget.org/packages/TypeSignature.Extensions.DependencyInjection/" target="_blank">TypeSignature.Extensions.DependencyInjection</a> nuget package

```csharp
// setup our DI
var serviceProvider = new ServiceCollection()
  .AddTypeSignatureSHA256()
  .BuildServiceProvider();

// resolve SignatureBuilder
ISignatureBuilder signatureBuilder = serviceProvider.GetService<ISignatureBuilder>();

string signature = signatureBuilder.GetSignature<Person>();
```
