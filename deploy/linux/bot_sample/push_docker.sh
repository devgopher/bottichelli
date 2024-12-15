# This is a sample script
function check_and_setup()
{
  read -p "Enter $1 $2: " $1
}

rm -rf /tmp/tmp_docker_botticelli
mkdir /tmp/tmp_docker_botticelli

pushd Botticelli

check_and_setup http_port "(example:80)"
check_and_setup https_port "(example:8080)"
check_and_setup server_uri "(example:http://localhost:53635/v1/)"
check_and_setup analytics_uri "(example:http://localhost:5251/v1/)"
                                                                 
cp Dockerfile /tmp/tmp_docker_botticelli/

sed -i "s/\$http_port/$http_port/g" /tmp/tmp_docker_botticelli/Dockerfile
sed -i "s/\$https_port/$https_port/g" /tmp/tmp_docker_botticelli/Dockerfile
sed -i "s/\$server_uri/$server_uri/g" /tmp/tmp_docker_botticelli/Dockerfile
sed -i "s/\$analytics_uri/$analytics_uri/g" /tmp/tmp_docker_botticelli/Dockerfile
                                    
docker build --tag "botticelli_bot_sample_dev:0.4" . --no-cache

#docker image push --all-tags <registry-host:5000/myname/myimage>