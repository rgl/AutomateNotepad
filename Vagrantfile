# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.require_version ">= 1.8.1"

Vagrant.configure("2") do |config|
    config.vm.box = "windows_2012_r2"

    config.vm.provider :virtualbox do |v, override|
        v.linked_clone = true
        v.cpus = 2
        v.memory = 2048
        v.customize ["modifyvm", :id, "--vram", 64]
        v.customize ["modifyvm", :id, "--clipboard", "bidirectional"]
    end

    config.vm.provision "shell", path: "provision.ps1"
end
