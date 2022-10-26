import sqlite3
import sys
import string
import random


con = sqlite3.connect("db.sqlite")

cur = con.cursor()

for i in range(int(sys.argv[1])):
    
    street=''.join(random.choices(string.ascii_uppercase+ string.ascii_lowercase  + string.digits, k=10))
    housenumber=''.join(random.choices(string.digits, k=3))
    zip=''.join(random.choices(string.ascii_uppercase + string.digits+' ', k=10))
    city=''.join(random.choices(string.ascii_uppercase, k=6))
    country=''.join(random.choices(string.ascii_uppercase, k=6))
    query ="INSERT INTO Address(street,housenumber,zipcode,city,country) values('{}','{}','{}','{}','{}');".format(street,housenumber,zip,city,country)
    print(query)
    cur.execute(query)
    

# Be sure to close the connection
con.commit()
con.close()