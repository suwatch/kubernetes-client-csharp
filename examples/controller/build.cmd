setlocal
cd /d C:\gitroot\kubernetes-client-csharp\examples\controller
call dotnet restore
call dotnet publish -c Debug

exit /b 0

REM build image
docker build -t suwatch01/kubecontroller-csharp .
docker push suwatch01/kubecontroller-csharp:latest

REM apply crd
kubectl apply -f C:\gitroot\kubernetes-client-csharp\examples\controller\definitions\crd.yaml
kubectl get crd
kubectl delete crd examples.engineerd.dev

REM run controller
kubectl delete deployment kubecontroller-csharp
kubectl get all
kubectl apply -f C:\gitroot\kubernetes-client-csharp\examples\controller\definitions\kubecontroller-csharp.yaml
kubectl logs --tail=100 -l app=kubecontroller-csharp
kubectl exec -it kubecontroller-csharp-6695d898cc-z4gtd --container kubecontroller-csharp -- /bin/bash

REM apply custom resource
kubectl apply -f C:\gitroot\kubernetes-client-csharp\examples\controller\definitions\cr.yaml
kubectl edit -f C:\gitroot\kubernetes-client-csharp\examples\controller\definitions\cr.yaml
kubectl get examples.engineerd.dev
kubectl get examples.engineerd.dev my-cr-example
kubectl edit examples.engineerd.dev my-cr-example
kubectl delete examples.engineerd.dev my-cr-example
