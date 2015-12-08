# Iodine Programming Language
[![Build Status](https://travis-ci.org/IodineLang/Iodine.svg)](https://travis-ci.org/IodineLang/Iodine)

Iodine is dynamically typed multi-paradigm programming language written in C#. The syntax of the Iodine is derived from several languages including Python, C#, and F#

#### Usage
To install on *NIX systems simply run the ```install.sh``` shell script as root which will install Iodine to /usr/bin/iodine. Windows users must install manually. The executable in ```bin``` can be invoked directly, however some modules may not be present unless you copy the ```modules``` directory into bin. Alternatively, Windows users may download an MSI installer from the Release Tab.

A file can be ran by invoking the interpreter as such
```
iodine myFile.id
```

#### Example
Below is a Hello, World program in Iodine. You can find more examples in the examples directory
```go
func main (args) {
    print ("Hello, World!");
}
```
