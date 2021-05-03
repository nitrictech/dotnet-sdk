FROM node:latest

RUN npm install -g license-check-and-add

COPY ./license_header.txt /license_header.txt
COPY ./licenseconfig.json /licenseconfig.json

WORKDIR /data
VOLUME /data

CMD ["license-check-and-add", "add", "-f", "/licenseconfig.json"]