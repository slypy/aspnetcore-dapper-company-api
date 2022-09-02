#!/bin/sh

# loop over the result of 'ls -1 *.sql'
#     'ls -1' sorts the file names based on the current locale 
#     and presents them in a single column
for i in *.sql; do 
    sqlcmd -S 192.168.1.8,4066 -U SA -P S-123123s -i $i
done

# TODO: Test for find command if applicable