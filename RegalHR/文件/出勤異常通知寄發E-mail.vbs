Call OpenURL()
Sub OpenURL()
'Force the script to finish on an error.
On Error Resume Next


'Declare variables
Dim objRequest
Dim URL
Set objRequest = CreateObject("Microsoft.XMLHTTP")


'Put together the URL link appending the Variables.
URL = "http://192.168.0.12:81/HRMail/AbnSendMail?key=89947155"


'Open the HTTP request and pass the URL to the objRequest object
objRequest.open "POST", URL , false


'Send the HTML Request
objRequest.Send


'Set the object to nothing
Set objRequest = Nothing
End Sub