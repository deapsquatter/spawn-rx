#!/bin/sh
echo "Nice to meet you from the standard out.My name is"
echo "Hello from standard err!.My Surname is" 1>&2
echo " John"
echo " Smith" 1>&2
exit 0