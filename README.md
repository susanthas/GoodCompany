# GoodCompanyChallenge
Thank you very much for this opportunity to show my development skills. I started learning react on 07/Mar/2021 for this
project, and I'm happy where I'm after 1 week of using it. :).

Main Components 
----------------
ER.png - This is a bit complex than the 
solution developed. But in a database driven application, this would be the right approach.

cinventory.web.sln - contains web app (cinventory.web) done using react/ .net Core and test project (cinventory.test).

Eventhough I like to follow, standards I also like to make things easier for developers 
and to manage. I prefer all my APIs to return a JsonResponse(code, message, data) object, so 
that it's easier to handle the response. When the code is less than 0, I would know the call
failed somewhere, and the message would tell me what went wrong. If the code is greater than 0,
I would know the call was success and I received what I expected. The data would always contain 
relevent data for pass or fail response.
