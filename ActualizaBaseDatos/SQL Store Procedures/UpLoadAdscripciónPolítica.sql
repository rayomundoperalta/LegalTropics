﻿IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = N'UpLoadAdscripciónPolítica')
BEGIN
   DROP PROCEDURE [dbo].[UpLoadAdscripciónPolítica]
END 
go
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpLoadAdscripciónPolítica]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM [dbo].[AdscripciónPolítica]
END
GO