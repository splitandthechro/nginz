# Iodine core library
Module: `core.id`

## Overview
- [core.printf](#printf)
- [core.printr](#printr)
- [core.sprintf](#sprintf)
- [core.sprintf_t](#sprintf_t)
- [core.format](#format)
- [core.len](#len)
- [core.repr](#repr)
- [core.hex](#hex)
- [core.reverse](#reverse)

## printf <a id="printf"></a>
Formats and prints a `Str`

### Description
```python
Null printf (Object format, Variadic args)
```

### Parameters
`format`:  
The format of the output  

`args`:  
Parameter list

### Syntax
Refer to [sprintf_t](#sprintf_t-syntax)

## printr <a id="printr"></a>
Prints a user-friendly representation of an object using [core.resp](#resp)

### Description
`Null printr (Object obj)`

### Parameters
`obj`:  
Any `Object` that needs to be printed out nicely.

## sprintf <a id="sprintf"></a>
Formats and returns a `Str`

### Description
```python
String sprintf (Object format, Variadic args)
```

### Parameters
`format`:  
The format of the output

`args`:  
Parameter list

### Syntax
Refer to [sprintf_t](#sprintf_t-syntax)

## sprintf_t <a id="sprintf_t"></a>
Formats and returns a `Str`

### Description
```python
String sprintf (Object format, List|Tuple args)
```

### Parameters
`format`:  
The format of the output

`args`:  
A `Tuple` or a `List` containing the arguments

### Syntax <a id="sprintf_t-syntax"></a>
`sprintf_t` can operate in two different modes:  
- Sequential: Access to the arguments using `{}`
- Indexed: Access to the arguments by index using `{i}`

Sequential syntax:
```python
# Prints: Hello, world!
sprintf_t ("{}, {}!", ("hello", "world"))

# Prints: Tuple: (1, 2, a, b)
sprintf_t ("Tuple: {}", ((1, 2, "a", "b")))
```
Indexed syntax:
```python
# Prints: Hello, world!
sprintf_t ("{0}, {1}!", ("hello", "world"))
```
Mixed syntax:
```python
# Prints: Hello, Hello!
sprintf_t ("{0}, {}!", ("hello", "world"))

# Prints Hello, world!
sprintf_t ("{}, {1}!", ("hello", "world"))
```

## format <a id="format"></a>
Formats and returns a `Str`

### Description
```python
String sprintf (Object format, Variadic args)
```

### Parameters
`format`:  
The format of the output

`args`:  
Parameter list

### Syntax
Refer to [sprintf_t](#sprintf_t-syntax)

## len <a id="len"></a>
Returns the size of an `Object`

### Description
```python
Int len (Object obj)
```

### Parameters
`obj`:  
Any `Object`

### Examples
```python
a = { "x", "y", "z" }

# Returns the Int 3
len (a)

# Returns the Int 13
len ("Hello, World!")
```

## repr <a id="repr"></a>
Returns a user-friendly representation of an `Object`

### Description
```python
String repr (Object obj)
```

### Parameters
`obj`:  
Any `Object`

### Examples
```python
a = ( 1, 2, 3, "a", "b", "c" )

# Returns the Str "Tuple"
Str (a)

# Returns the Str "(1, 2, 3, a, b, c)"
repr (a)

b = HashMap ()
b["a"] = 25
b["b"] = 31

# Returns the Str "HashMap"
Str (b)

# Returns the Str "{a: 25, b: 31}"
repr (b)
```

## hex <a id="hex"></a>
Returns the hex representation of an `Int`

### Description
```python
String hex (Int n)
```

Info:  
`hex` returns just the base 16 value of `n`, without prepending `0x`

### Parameters
`n`:  
Any `Int`

### Examples
```python
# Returns: 1A
hex (26)

# Returns: 4BC
hex (1212)

# Returns the Str "0xFFFF"
sprintf ("0x{}", hex(65535))
```

## reverse <a id="reverse"></a>
Iterates over an iterable `Object` and return its items in reversed order.

### Description
```python
Mixed reverse (Mixed iterable)
```

### Parameters
`iterable`:  
Any iterable `Object`, like a `List` or a `Tuple`

### Examples
```python
lst = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }

# Returns the List { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }
reverse (lst)

tpl = ( 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 )

# Returns the Tuple ( 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 )
reverse (tpl)

str = "Hello, World!"

# Returns the Str "!dlrow ,olleH"
reverse (str)
```
