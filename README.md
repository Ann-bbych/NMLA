##NMLA
Numerical Methods of Linear Algebra

A collection of C# console applications developed during university coursework in numerical linear algebra. The projects cover solving linear systems, matrix factorization, and eigenvalue approximation, with algorithms implemented directly using arrays and the .NET standard library.

## Implemented Methods

- **Gaussian elimination** — solves linear systems using partial pivoting and back substitution, computes the determinant, and checks the solution against the original equations.
- **LU factorization** — constructs lower and upper triangular factors without pivoting, solves the resulting triangular systems, and verifies both the factorization and the solution.
- **Jacobi iteration** — checks diagonal dominance, iteratively approximates the solution, and reports the iteration count and residual.
- **Power method** — uses normalized iteration and component ratios to approximate a dominant eigenvalue and its eigenvector, with an eigenpair residual check.
- **Thomas algorithm** — solves tridiagonal systems using a left sweep and includes a fixed finite-difference example of a boundary-value problem with a known exact solution.

## Features

- Text-file input and reports written to both the console and output files.
- Intermediate calculations, method-specific input checks, and numerical verification.
- Ukrainian console messages and reports.

## Technologies

C# · .NET 10 · VS Code
