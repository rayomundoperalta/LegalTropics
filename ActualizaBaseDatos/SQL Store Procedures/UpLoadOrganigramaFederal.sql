﻿IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = N'UpLoadOrganigramaFederal')
BEGIN
   DROP PROCEDURE [dbo].[UpLoadOrganigramaFederal]
END 
go
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpLoadOrganigramaFederal]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM [dbo].[OrganigramaFederal]
END
GO