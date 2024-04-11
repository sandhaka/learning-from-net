
// Source article:
// https://codinghelmet.com/articles/understanding-covariance-and-contravariance-of-generic-types-in-cs

// Variance in programming

Base a = new Base();
Base b = new Derived();

a.DoSomething();
b.DoSomething();

var z = new Derived();

z.DoSomething();
z.DoSomethingMore();


// This is how the object substitution principle works on regular types.
// These four possibilities exists when we are using a reference to the
// base type and a reference to a derived type.

// Variance can be declared on interfaces and delegate types in C#. 
// Variance is augmenting the object substitution principle to apply to
// generic types following the same principles as in non-generic classes.

// When speaking of interfaces - generic or not - they can either
// produce or consume objects.
// That will be very important when speaking about variance.

// The out keyword indicate the covariant type T. The interface produce
// an object of type T.
// The int keyword indicate the contravariant type T. The interface
// accept the object parameter of type T.

IProducer<Base> prodOfBase = null!;
Base aa = prodOfBase.Produce();
// Illegal: Derived bb = prodOfBase.Produce();
// That violate to object substitution principle.

IProducer<Derived> prodOfDerived = null!;
Derived bb = prodOfDerived.Produce();
Base c = prodOfDerived.Produce();

// The producing interfaces are behaving covariantly.
// They are moving in the same direction. You derive a class,
// a generic parameter class, one from another, and generic interfaces
// of these two classes behave as if they were derived from one another
// in the same direction.

// Start using consumer interface

IConsumer<Base> consOfBase = null!;
consOfBase.Consume(new Base());
consOfBase.Consume(new Derived());

IConsumer<Derived> consOfDerived = null!;
consOfDerived.Consume(new Derived());
// No: consOfDerived.Consume(new Base());

// We cannot pass the base object in the consumer interface of the derived type,
// because base object cannot be assigned to the method argument excepting derived.
// That is how the method argument is declared, so the last invocation above
// would violate the object substitution principle.

IProducer<Base> p = prodOfBase;            // IProducer<Base>
IProducer<Base> q = prodOfDerived;         // IProducer<Derived>
IProducer<Derived> r = prodOfDerived;      // IProducer<Derived>
// No: IProducer<Derived> s = prodOfBase;  // IProducer<Base>

IConsumer<Derived> t = consOfDerived;      // IConsumer<Derived>
IConsumer<Derived> u = consOfBase;         // IConsumer<Base> -> contravariant usage
IConsumer<Base> v = consOfBase;            // IConsumer<Base>
// No: IConsumer<Base> w = consOfDerived;  // IConsumer<Derived>

file class Base
{
    public void DoSomething() =>
        Console.WriteLine($"Do from: ${GetType().Name}");
}

file class Derived : Base
{
    public void DoSomethingMore() =>
        Console.WriteLine($"Do more from: ${GetType().Name}");
}

// Declared as variant
file interface IProducer<out T>
{
    T Produce();
}

// Declared as contravariant
file interface IConsumer<in T>
{
    void Consume(T obj);
}