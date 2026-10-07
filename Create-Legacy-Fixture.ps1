param([Parameter(Mandatory=$true)][string]$Output)
$ErrorActionPreference = 'Stop'
$values = New-Object 'System.Collections.Generic.List[object]'
$german = [Globalization.CultureInfo]::GetCultureInfo('de-DE')
# Populate legacy cache fields; cold cultures alone do not cover real settings.
$null = $german.TextInfo.ToUpper('test')
$null = $german.CompareInfo.Compare('a', 'b')
$null = $german.NumberFormat.CurrencySymbol
$null = $german.DateTimeFormat.ShortDatePattern
$null = $german.Calendar.TwoDigitYearMax
$values.Add($german)
$values.Add([Globalization.CultureInfo]::InvariantCulture)
$text = [Globalization.CultureInfo]::GetCultureInfo('tr-TR').TextInfo.Clone()
$text.ListSeparator = '|'
$values.Add($text)
$values.Add([Globalization.CultureInfo]::GetCultureInfo('de-DE').CompareInfo)
$number = $german.NumberFormat.Clone()
$number.NumberDecimalSeparator = '~'
$values.Add($number)
$date = $german.DateTimeFormat.Clone()
$date.ShortDatePattern = 'yyyy/MM/dd'
$values.Add($date)
$calendar = New-Object Globalization.GregorianCalendar
$calendar.TwoDigitYearMax = 2099
$values.Add($calendar)
$formatter = New-Object System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
$stream = [IO.File]::Create($Output)
try { $formatter.Serialize($stream, $values) } finally { $stream.Dispose() }
