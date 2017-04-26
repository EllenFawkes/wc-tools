#!/bin/bash

echo "Creating WildFly admin pipe ..."
ssh -L 1234:localhost:9990 konosadmin@10.22.22.218
