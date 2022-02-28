@echo off
set ProjectName=%1%
set Version=%2%

echo %ProjectName%
echo %Version%

git subtree split --prefix=Assets/%ProjectName% --branch upm
git tag %Version% upm
git push origin upm --tag
cd Assets/%ProjectName%
npm publish --registry http://npm.wingjoy.cn