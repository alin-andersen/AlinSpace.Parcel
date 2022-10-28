<img src="https://github.com/onixion/AlinSpace.Parcel/blob/main/Assets/Icon.png" width="200" height="200">

# AlinSpace
[![NuGet version (AlinSpace.Parcel)](https://img.shields.io/nuget/v/AlinSpace.Parcel.svg?style=flat-square)](https://www.nuget.org/packages/AlinSpace.Parcel/)

General file format.

## Why?

With this library you will never ever have to build any custom file formats anymore. The dynamic parcel format can contain any kind of data.

## Examples

Lets create a new parcel:

```csharp
using var parcel = Parcel.New();

// Add file.
parcel.AddFile("Image.png", "Desktop/Image.png");

// Add any text.
parcel.CopyFile("MyText", "123");

// Add JSON objects.
parcel.AddJson<MyObject>("MyObject");

// Resets the whole parcel.
parcel.Reset();

// Delets the text.
parcel.Delete("MyText");
```

Then pack and output the parcel file:

```csharp
parcel.Pack("MyParcel.parcel");
```

This packs the parcel into a single file.

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
