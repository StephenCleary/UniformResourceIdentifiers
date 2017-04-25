To define a new URI type:

1. Define scheme as static readonly property (must pass Util.IsValidScheme, i.e., lowercase).
2. Inherit "ComparableBase, IURI", using normalizing components.
3. Override ToString.
4. Provide stronger-typed Resolve.
5. Define Factory.
6. Static methods: Register, Parse.
