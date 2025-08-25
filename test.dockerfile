FROM alpine:latest
WORKDIR /src
COPY . .
RUN find . -name "*.csproj" -type f
