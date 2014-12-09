// MATRIX EIGENVALUES
A=diag([1,2,3]);
X=rand(3,3);
A=inv(X)*A*X;
spec(A)

x=poly(0,'x');
pol=det(x*eye()-A)
roots(pol)

[S,X]=bdiag(A);
clean(inv(X)*A*X)

// PENCIL EIGENVALUES
A=rand(3,3);
[al,be,R] = spec(A,eye(A));
al./be
clean(inv(R)*A*R)  //displaying the eigenvalues (generic matrix)
A=A+%i*rand(A);
E=rand(A);
roots(det(A-%s*E))   //complex case
