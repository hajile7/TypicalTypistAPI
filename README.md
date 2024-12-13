About
-----
The backend for TypicalTypist, including SQL file for database creation & accompanying API.

To Contribute
-------------
To run the project locally, first create the database by running the SQL script. After running the script and ensuring the DB has been created correctly (I'd just double check the words table to make sure it's been populated), head into the 'PopulateDatabasePrograms' folder and run both the programs contained within (order does not matter, though I normally populate bigraphs first, followed by the space bigraphs). Once you've done that, run a SELECT * FROM query in your local database instance on the bigraphs table to ensure they were created correctly (there should be 6816 bigraphs if this was done correctly). If words & bigraphs tables are populated, you should be all set on the local database and ready to get started. 
