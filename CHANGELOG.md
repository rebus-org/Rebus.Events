# Changelog

## 2.0.0-beta01
* First version

## 3.0.0
* Update to Rebus 3

## 4.0.0
* Update to Rebus 4
* Add .NET Core support (netstandard1.6)

## 4.0.1
* Fix bug that would pass an un-initialized `IBus` instance field to the event listeners

## 4.0.2
* Fix bug that would call event callbacks multiple times if the `.Events(..)` configuration had been applied more than once - thanks [dariogriffo]

## 5.0.0
* Update packages and modernize a bit

[dariogriffo]: https://github.com/dariogriffo