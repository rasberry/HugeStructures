# HugeStructures
Disk-backed structures that can be larger than available RAM

## Overview
### TitanicArray classes
These classes are designed to emulate a regular array and have the following features:
* they are a fixed length
* data can be accessed in a random manner
* the arrays are Read/Write
* all extend the ITitanicArray\<T\> interface

## TitanicFileArray
A basic disk-backed array using just a [file](https://msdn.microsoft.com/en-us/library/system.io.file.aspx) and and [LRU](https://en.wikipedia.org/wiki/Cache_replacement_policies#LRU) based cache.

## TitanicMMFArray
This array is backed by a [MemoryMapped](https://docs.microsoft.com/en-us/dotnet/standard/io/memory-mapped-files) file. Pretty much just a wrapper around the [MemoryMappedFile](https://msdn.microsoft.com/en-us/library/system.io.memorymappedfiles.memorymappedfile.aspx) class

## TitanicSQLiteArray
This array is backed by a [SQLite](https://sqlite.org/) database.

## TitanicLiteDBArray
This array is backed by a [LiteDB](http://www.litedb.org/) database.

## TitanicRaptorDBArray
This array is backed by a [RaptorDb](https://www.codeproject.com/Articles/316816/RaptorDB-The-Key-Value-Store-V2) database.

## TitanicIMArray
This array is backed by [Magick.NET](https://github.com/dlemstra/Magick.NET)
<br/>Note: TitanicIMArray sometimes has a problem converting data to float values (the internal storage type). So this array might be unreliable.

## Performance
Testing on my machine using 2^20 doubles (4MiB of data) results in these times:

* TitanicMMFArray
  * Random Sequence: 3s
  * Linear Sequence: 2s
* TitanicIMArray
  * Random Sequence: 10s
  * Linear Sequence: 9s
* TitanicRaptorDBArray
  * Random Sequence: 13s
  * Linear Sequence: 12s
* TitanicFileArray
  * Random Sequence: 25s
  * Linear Sequence: 12s
* TitanicSQLiteArray
  * Random Sequence: 345s
  * Linear Sequence: 294s
* TitanicLiteDBArray
  * Random Sequence: ?s (took too long)
  * Linear Sequence: 570s
