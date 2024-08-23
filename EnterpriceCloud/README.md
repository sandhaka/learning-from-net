<samp>

Cloud Native Enterprise Project - State Of Art Sample
> Minimal project updated with state-of-the-art **personal** choices and technologies.


#### Use Case
Enterprise message handler waiting for data to process. 

#### Caveats
- I prefer map domain object to persistence layer manually. Mapping libraries have some issues on working with
record types due to the absence of a default parameterless constructor. I don't want to use it in favor of 
immutable design in db models namespace. You can find mapping logic implemented as implicit conversion operators 
in the db models classes.

#### Components


</samp>