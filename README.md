Simplic CDN CSharp Driver
===

This repository contains the .net / csharp driver to work with the simplic cdn in local networks and in the cloud.
In the following article the basic usage will be explained.

## Samples

This is a sample how to use the api.

### Open a new connection

For opening a new connection, just create a `Cdn` instance and pass and url.

```csharp
using (var cdn = new Cdn("http://localhost:50121/api/v1-0/"))
{
    // Ping the server
    Console.WriteLine("Ping: " + cdn.Ping());
}
```

If you want to test the connection, just `Ping()` it.

Now we just need to authenticate to do further things:

```csharp
using (var cdn = new Cdn("http://localhost:50121/api/v1-0/"))
{
    // Ping the server
    Console.WriteLine("Ping: " + cdn.Ping());

	// Try to connect
	if (cdn.Connect("<<username>>", "<<password>>"))
    {

	}
}
```

Now we are ready to read and write data.

### Write Data

### Read Data


