dotnet ef dbcontext scaffold  "data source=112.213.88.49,1500;initial catalog=LocatingApp;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;" Microsoft.EntityFrameworkCore.SqlServer -c DataContext  -o Models -f --no-build --use-database-names --json