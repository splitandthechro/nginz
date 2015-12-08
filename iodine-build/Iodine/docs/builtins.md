# Iodine Builtin Functions

##### func ```print``` (object)
Prints the string representation of an object
##### func ```input``` ([prompt])
Reads from the standard input stream, optionally displaying prompt
##### func ```eval``` (source)
Evaluates a string of iodine source code
##### func ```filter``` (iterable, function)
Iterates through an iterable object, passing each iteration to function. If function returns true, then the element is added to a list that is returned to the caller.
##### func ```map``` (iterable, function)
Iterates through an iterable object, performing function on each iteration. The outputs from function is added to a new list that is returned to the caller.
##### func ```reduce``` (iterable, function)
Reduces all members of an iterable object by applying function to each item left to right. The function passed to reduce receives two arguments, the result of the last call to function, and the current item from the iterable object.  
##### func ```range``` (n)
##### func ```range``` (start, end)
Returns an iterable object with n iterations 
##### func ```open``` (file, mode)
Opens up a file, returning a new stream object.
# Iodine Builtin Classes
##### class ```Int``` (object)
Class represents a 64 bit signed integer. 
##### class ```Char``` (object)
Class represents a UTF-16 char
##### class ```Bool``` (object)
Class represents a boolean
##### class ```ByteArray``` (object)
Class represents a fixed length array of bytes
##### class ```ByteStr``` (object)
Class represents an immutable string of bytes
##### class ```Str``` (object)
Class represents a string
##### class ```HashMap``` ([object])
Class represents a HashMap (Dictionary). An optional list containing key/value pairs can also be passed to initialize the dictionary.
```
myDict = HashMap ();
myDict = HashMap ({("key", "value")});
```
##### class ```List``` (p[object])
Class represents a variable length list
##### class ```Tuple``` ([object])
Class represents a tuple. A tuple can be initialized by passing any iterable object to Tuple's constructor
##### class ```Event``` ()
Class represents an event. An instance of the event class is callable. Calling an instance of the ```Event``` class will call all registered events. Events can be registered with the ```+=``` operator.
##### class ```Stream``` ()
Class represents a stream. This class can not be directly instantiated however is returned from the ```open``` function.
##### class ```Exception``` (message)
Class represents a generic exception. Exceptions in Iodine can be raised using the ```raise``` keyword. Any class deriving ```Exception``` can be raised.
Example:
```
try {
    raise Exception ("An exception!");
} except (ex) {
}
```

