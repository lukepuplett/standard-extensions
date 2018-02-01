# Introduction 
This is a repository for a NuGet package. A library of general extension methods for many common uses across any .NET project.

# Getting Started
There's nothing to it:
1.	Reference the package
2.	Use types the new types in namespaces under the Centrica EMT root
3.	Profit!

# Build and Test
Please ensure that any and all classes you add to this package are very well tested. This is a general package intended for use across many projects. Code must be of the highest quality. 

Note: all extension method static classes are in the System namespace so as to make them always available to programmers without having to add a particular Using statement. This means care must be taken not to conflict with existing members on the .NET types being extended.

Note: Extension methods are defined in static classes that are named StandardExtensions_{namespace for the types being extended}Extensions.cs

# Contribute
Add any generally useful data type in the same vein as the types already in the package.

Remember to update the .nuspec file as needed.