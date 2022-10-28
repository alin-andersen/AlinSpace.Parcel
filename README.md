<img src="https://github.com/onixion/AlinSpace.Parcel/blob/main/Assets/Icon.png" width="200" height="200">

# AlinSpace.Parcel
[![NuGet version (AlinSpace.Parcel)](https://img.shields.io/nuget/v/AlinSpace.Parcel.svg?style=flat-square)](https://www.nuget.org/packages/AlinSpace.Parcel/)

Dynamic parcel file format.

## Why?

With this library you will never ever have to build any custom file formats anymore. The dynamic parcel format can contain any kind of data.

## Features

- Versioning support
- Metadata support
- JSON support
- Add any file to the parcel
- Parcel is compressed (ZIP)
- Parcel can be unpacked by un-zipping it (you can open it and do stuff without needing the library at all)

## How?

When creating a new parcel a tempoary random directory called the **workspace** is created. This directory is used as a temporarly place to keep the files open when working with the parcel file. When the parcel is disposed, this workspace directory will be deleted. 

## Examples

Lets create a new parcel:

```csharp
using var parcel = Parcel.New();

// Add file.
parcel.CopyFile("Image.png", "Desktop/Image.png");

// Add any text.
parcel.WriteText("MyText", "123");

// Add JSON objects.
parcel.WriteJson<MyObject>("MyObject");

// Delets the text.
parcel.Delete("MyText");

// Resets the whole parcel.
parcel.Reset();
```

Then pack and output the parcel file:

```csharp
parcel.Pack("MyParcel.parcel");
```

This packs the parcel into a single file.
You can also unpack a parcel file:

```csharp
parcel.Unpack("MyParcel.parcel");
```

## Examples Metadata

The parcel format already contains metadata:

```csharp
parcel.Metadata["Author"] = "Alin Andersen";
```

## Examples Versioning

Versioning is also integrated:

```csharp
using var parcel = Parcel.Open("MyParcel.parcel", NeedsVersion.HigherThan("1.0.2"));
```

## Examples Unpacking

You can open one parcel file and unpack another parcel file into it:

```csharp
using var parcel = Parcel.Open("MyParcel.parcel");

// Unpacks the other parcel file into the parcel and overwrites the resources.
parcel.Unpack("AnotherParcel.parcel");
```

Unpacking a parcel file is the same as open a parcel file:

```csharp
using var parcel = Parcel.Open("MyParcel.parcel");

// Same as above.
using var parcel = Parcel.New();
parcel.Unpack("MyParcel.parcel");
```

