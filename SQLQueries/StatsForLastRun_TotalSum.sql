SELECT FORMAT(SUM(Uploaded)/ (1024.0 * 1024), 'N3') as 'TotalSumMB'
FROM TorrentStats.dbo.StatsForLastRun