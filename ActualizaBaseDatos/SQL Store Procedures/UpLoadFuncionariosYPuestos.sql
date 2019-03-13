IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = N'UpLoadFuncionariosYPuestos')
BEGIN
   DROP PROCEDURE [dbo].[UpLoadFuncionariosYPuestos]
END 
go
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpLoadFuncionariosYPuestos]
AS
BEGIN
	SET NOCOUNT ON;
	select * From [dbo].[Funcionarios] INNER JOIN [dbo].[Puestos] ON [dbo].[Funcionarios].ID = [dbo].[Puestos].ID order by [dbo].[Funcionarios].Id1, [dbo].[Puestos].Id1
END
GO