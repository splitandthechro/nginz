# MySQL Module (`mysql`)
The MySQL module provides bindings to the MySQL database. It supports queries returning rows and non-queries. Scalars are not currently supported.

## Example
See [this example](https://gist.github.com/Aurora0000/b81093da3c201c61ddec) for a full demonstration of the `mysql` module.

## Module Functions
### `openDatabase(connectionString)`
Opens the database using the specified connection string. Connection strings look like this:

    server=localhost;userid=username; password=****;database=dbname
    

## MySQLConnection Functions
### `createParam(name, value)`
Returns a `MySQLParameter` object that can be used with prepared statements. In your query, replace the value you want to enter with @paramName. For example:

    INSERT INTO Persons VALUES (@id, @firstName, @lastName)
    


### `executeSql(query)`
Executes the specified SQL. Unsafe for use with user input. Instead, use `executeSqlPrepared()`.

`executeSql()` does **not** return any output.

### `querySql(query)`
Executes the specified SQL and returns a list of dictionaries representing the column names and values for each row. Unsafe for use with user input. Instead, use `querySqlPrepared()`.

### `executeSqlPrepared(query, param1, param2...)`
Executes the specified `query` with the parameters. An unlimited number of parameters can be used. Parameters are created using `createParam(name, value)` from a database object.

### `querySqlPrepared(query, param1, param2...)`
Runs `querySql()` with prepared statements as in `executeSqlPrepared()`.

### `close()`
Closes the open database object.

