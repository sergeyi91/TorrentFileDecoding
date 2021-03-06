SELECT [Name]
      ,FORMAT([Uploaded] / (1024.0 * 1024), 'N3') as 'UploadedMB'
      ,[AddedOn]
      ,[TorrentName]
      ,[TrackerId]
      ,[CreatedOn]
FROM [TorrentStats].[dbo].[StatsForLastRun]
WHERE [TrackerId] = '' OR [TrackerId] LIKE '%lab%'
ORDER BY [Uploaded] DESC
--ORDER BY [CreatedOn] ASC
