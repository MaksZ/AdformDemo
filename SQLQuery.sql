/*  Usage: 
 *  1) run this script to get the result 1
 *  2) uncomment lines in query and run again to get the result 2
 */

WITH Totals_CTE(ClientId, Total)
AS
(
	SELECT c.[ClientID], sum(o.[Total])
	  FROM [dbo].[users] c
	  FULL JOIN [dbo].[orders] o on o.[UserId] = c.[UserId]
	GROUP BY c.[ClientID]
)
SELECT 
 -- TOP 2
	ISNULL(STR(ClientId), 'n/a') as client,
	ISNULL(Total, 0) as total
FROM Totals_CTE
 -- ORDER BY 2 DESC