# Advanced-Programming-Final-Term-Exam-2017
Advanced Programming  Final Term Exam project in 2017

## Exercise 1
Design a set of classes to represent annotated code.
## Exercise 2
Implement a recursive descent parser for annotated interface declarations. Ensure that each parser method returns
a proper object representing the input it has analyzed.
## Exercise 3
Write a code generator for producing plain code for each interface without the annotations. Write a generator
that produces a corresponding SQL schema for each interface.
Ensure to use polymorphism whenever possible.
## Exercise 4
Design and implement an IEntityManager class, providing the following interface:
interface IEntityManager<T> {
 void persist(T entity);
 void remove(T entity);
 T find(Object primaryKey);
}
Method persist() serializes the entity and inserts it in the appropriate tables. For example, an instance of
Book must store a row in table Book as well as one in the in the Publisher table corresponding to the book
publisher attribute object. Method remove() instead will just perform the removal of the row in the Book
table.
Method find() should retrieve and deserialize the object Book with the given key as well as the
corresponding Publisher object from the Publisher table.
In order to access the database the IEntityManager should use suitable SQL queries: write a generator for
such queries.
Show the SQL queries produced for the example in the introduction.
## Exercise 5
Extend the IEntityManager interface to include the following method:
 Query<T> createQuery(String query);
where parameter query is a string expressing a SQL query and the Query implements this interface:
interface IQuery<T> {
 List<T> getResultList();
 void execute();
}
The method getResultList() performs the query and “hydrates” (i.e. deserializes into objects) the result
set into a list of objects.
The method execute() is used to perform update or delete SQL queries.
3
Show an example of the generated SQL query.
## Exercise 6
Explain what is an Object-Relational Mapping and discuss and compare Linq and JPA as techniques to facilitate
access to data in databases. In particular compare the expressiveness of logical operator available in Linq with
respect to those of SQL based ORMs.
