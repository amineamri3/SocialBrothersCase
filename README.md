
# Social Brothers Case

This project consists of developping a .NET WEB API 
This assignment consists of 3 parts:
- **Part 1**: API general
- **Part 2**: Filters/Queries
- **Part 3**: API calls


## Part 1
This part is the easiest part, it consists of mainly setting up the project and the database and creating .

What is used in this part:
- Regex for input validation
- Swagger for documentation

What i *should* used in this part:
- A DTO to seperate the user input model and the actual entity
## Part 2
This is by far the hardest part of this case, my understanding of Linq was pretty poor going into this assignement but after spending some time trying out solutions i actually learned some stuff!

Starting off i actually has no idea how to implement this in Linq on top of my head so the first solutions i came up with where 
- Loading the db and filtering/sorting by code (Pretty bad idea yea!)
- Building a raw SQL query from the entity (Easy solution but it's a workaround and admitting defeat)
- Actually **learning Linq** enough to implement this solution (which was partially done after hours of googling)

Note: I wasn't able to actually make a native Linq query (That is translatable to SQL) in time and so I opted to use a [package](https://dynamic-linq.net/) that adds some functionalites to Linq
 
What interested me was how Linq queries actually work and i decided to test their performance to see if it was worth upgrading from a raw SQL query to Linq and so i created 3 API Endpoints

- /code: loadup the db and search/sort by code
- /rawsql: generate a raw SQL query from the Entity proprieties and run it.
- /linq: using dynamic linq


To actually compare these 3 solutions i had to generate a large dataset and so i wrote a python script *dbdump.py* that inserts random data into our sqlite database, and another script *benchmark.py* that sends GET requests to those routes with random input.

To try to make this benchmark as accurate as i can i sent the same exact input to all 3 Endpoints and i sent 50 request to try eleminating outside factors.
This benchmark is still far from acurate but it can give us some insight.

In this test I first loaded 1,000,000 entries into the database by running the first script 

```
  python dbdump.py 1000000
```
I then started up the webserver and the benchmark by running the second script

```
  python benchmark.py
```

And we get the following result (the total of 50 requests per endpoint):
```
  -----------Benchmark Ended------------
Endpoint /code   : a total of 353893.93ms  with an average of 7077.88ms per request
Endpoint /linq   : a total of 2494.31ms    with an average of 49.89ms   per request
Endpoint /rawsql : a total of 21468.30ms   with an average of 429.37ms  per request
```

The code solution is obviously out of the window, but what was surprising was how faster Linq runs the query compared to a raw SQL query.


#### What i *should* improve on:
-  Adding a native Linq solution
-  Optimizing the raw query and benchmarking again
-  Improve code readability and documentation

## Part 3

In this part we forward geocode our addresses and calculate the distance, Seemed pretty strightforward to me

What I *should* have done:
- Caching the geocoding API results or even better adding adding it directly to out Entity which help with API costs and/or Rate Limiting
- Handle API response codes better
- Sanetize and check API response body better
## How to run the project

To run this project you simpley execute the following command
```
cd SocialBrothersCase
dotnet run
```
The server will now be up and running and listening to:
- Port 5199 for HTTP
- Port 7199 for HTTPS

You can access the Swagger documentation by the following link: [https://localhost:7166/swagger/index.html](https://localhost:7166/swagger/index.html)

## Notes

- There will be 3 extra endpoints used for benchmark and can be ignored.
- API codes are not placed in a .env to make things simple
- All python scripts used and the database are on the project directory

And thank you!

