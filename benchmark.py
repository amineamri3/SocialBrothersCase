import requests
import random
import string
import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
url="http://127.0.0.1:5166"
routes=["/code","/linq","/rawsql"]
tries=50
sum = []
queries=[]

print("------------Starting Benchmark------------")
## generate random search query

for i in range(tries):
    queries.append(''.join(random.choices(string.ascii_uppercase, k=6)))
##fire dummy request to warm up tcp connection?

for route in routes:
    print("Benchmarking route "+route)
    sumtemp = False
    for i in range(tries):
        response = requests.get(url+route+'?search='+queries[i]+'?sortby=zipcode',  verify=False)
        print("Request {} completed in {}ms".format(i+1,response.elapsed.total_seconds() * 1000))
        if i == 0: sumtemp = response.elapsed
        else:sumtemp= sumtemp+response.elapsed 
    sum.append(sumtemp)



print("-----------Benchmark Ended------------")
print("Endpoint /code1  : a total of {:.2f}ms  with an average of {:.2f}ms per request".format(sum[0].total_seconds() * 1000,(sum[0]/50).total_seconds() * 1000))
print("Endpoint /linq   : a total of {:.2f}ms  with an average of {:.2f}ms per request".format(sum[1].total_seconds() * 1000,(sum[1]/50).total_seconds() * 1000))
print("Endpoint /rawsql : a total of {:.2f}ms  with an average of {:.2f}ms per request".format(sum[2].total_seconds() * 1000,(sum[2]/50).total_seconds() * 1000))