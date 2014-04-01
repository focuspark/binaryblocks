BinaryBlocks
============

A standard for data modelling and serializing data to persistent storage which is both backwards and forwards compatible. Using a form of markup, transient data can be persisted and retrieved reliably.

The source code comes in two parts. The first builds a binary which can consume the markup and convert it to code to be included in your project along with the second part, which is a library header file. No need for additional DLLs or external libraries. With a focus on .NET support, the first version is C# only with plans for C, CX, and VB.

Works on all CLR supported platforms. Generated code is compatible with .NET 2.0 and beyond.
