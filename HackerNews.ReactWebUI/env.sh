#!/bin/bash

if [ -d "$1" ]
then
  ENV_FILE=$1/env-config.js
else
  ENV_FILE=./env-config.js
fi

# Recreate config file
rm -rf $ENV_FILE
touch $ENV_FILE

# Add assignment 
echo "window._env_ = {" >> $ENV_FILE

# Read each line in .env file
# Each line represents key=value pairs
while read -r line || [[ -n "$line" ]];
do
  # Split env variables by character `=`
  if printf '%s\n' "$line" | grep -q -e '='; then
    varname=$(printf '%s\n' "$line" | sed -e 's/=.*//')
    varvalue=$(printf '%s\n' "$line" | sed -e 's/^[^=]*=//')
  fi

  if [[ $varname == '#'* ]]
  then
    continue
  fi

  # Read value of current variable if exists as Environment variable
  value=$(printf '%s' "${!varname}")
  # Otherwise use value from .env file
  [[ -z $value ]] && value=${varvalue}

  # check value is set correctly
  if [ ${#value} -eq 1 ] && { [[ "$value" == $'\n' ]] || [[ "$value" == $'\r' ]]; }; then
    value=""
  fi

  # Append configuration property to JS file
  echo "  $varname: \"$value\"," >> $ENV_FILE  
done < .env

echo "}" >> $ENV_FILE

chmod -v +x $ENV_FILE
echo "Completed writing to env file: $ENV_FILE"