Remove-Item .\main.json -Force  -ErrorAction Ignore
Remove-Item .\modules\*.json -Force  -ErrorAction Ignore
Remove-Item .\main.parameters.tmp.json -Force  -ErrorAction Ignore
Remove-Item .\parameters.tmp.json -Force  -ErrorAction Ignore