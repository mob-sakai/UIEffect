function UnityProgress (dom) {
	this.progress = 0.0;
	this.message = "";
	this.dom = dom;

	var parent = dom.parentNode;

	var background = document.createElement("div");
	background.style.background = Module["backgroundColor"] ? Module["backgroundColor"] : "#4D4D4D";
	background.style.position = "absolute";
	background.style.overflow = "hidden";
	parent.appendChild(background);
	this.background = background;

	if (Module["backgroundImage"])
	{
		var backgroundImg = document.createElement("img");
		backgroundImg.src = Module["backgroundImage"]; 
		backgroundImg.style.position = "absolute";
		backgroundImg.style.width = "100%";
		backgroundImg.style.height = "auto";
		backgroundImg.style.top = "50%";
		backgroundImg.style.transform = "translate(0, -50%)";
		background.appendChild(backgroundImg);
	}

	var logoImage = document.createElement("img");
	var splashStyle = Module["splashStyle"] ? Module["splashStyle"] : "Light";
	logoImage.src = "TemplateData/Logo." + splashStyle + ".png"; 
	logoImage.style.position = "absolute";
	parent.appendChild(logoImage);
	this.logoImage = logoImage;

	var progressFrame = document.createElement("img");
	progressFrame.src = "TemplateData/ProgressFrame." + splashStyle + ".png"; 
	progressFrame.style.position = "absolute";
	parent.appendChild(progressFrame);
	this.progressFrame = progressFrame;

	var progressBar = document.createElement("div");
	progressBar.style.position = "absolute";
	progressBar.style.overflow = "hidden";
	parent.appendChild(progressBar);
	this.progressBar = progressBar;

	var progressBarImg = document.createElement("img");
	progressBarImg.src = "TemplateData/ProgressBar." + splashStyle + ".png"; 
	progressBarImg.style.position = "absolute";
	progressBar.appendChild(progressBarImg);
	this.progressBarImg = progressBarImg;

	var messageArea = document.createElement("p");
	messageArea.style.position = "absolute";
	parent.appendChild(messageArea);
	this.messageArea = messageArea;


	this.SetProgress = function (progress) { 
		if (this.progress < progress)
			this.progress = progress; 
		this.messageArea.style.display = "none";
		this.progressFrame.style.display = "inline";
		this.progressBar.style.display = "inline";			
		this.Update();
	}

	this.SetMessage = function (message) { 
		this.message = message; 
		this.background.style.display = "inline";
		this.logoImage.style.display = "inline";
		this.progressFrame.style.display = "none";
		this.progressBar.style.display = "none";			
		this.Update();
	}

	this.Clear = function() {
		this.background.style.display = "none";
		this.logoImage.style.display = "none";
		this.progressFrame.style.display = "none";
		this.progressBar.style.display = "none";
	}

	this.Update = function() {
		this.background.style.top = this.dom.offsetTop + 'px';
		this.background.style.left = this.dom.offsetLeft + 'px';
		this.background.style.width = this.dom.offsetWidth + 'px';
		this.background.style.height = this.dom.offsetHeight + 'px';

		var logoImg = new Image();
		logoImg.src = this.logoImage.src;
		var progressFrameImg = new Image();
		progressFrameImg.src = this.progressFrame.src;

		this.logoImage.style.top = this.dom.offsetTop + (this.dom.offsetHeight * 0.5 - logoImg.height * 0.5) + 'px';
		this.logoImage.style.left = this.dom.offsetLeft + (this.dom.offsetWidth * 0.5 - logoImg.width * 0.5) + 'px';
		this.logoImage.style.width = logoImg.width+'px';
		this.logoImage.style.height = logoImg.height+'px';

		this.progressFrame.style.top = this.dom.offsetTop + (this.dom.offsetHeight * 0.5 + logoImg.height * 0.5 + 10) + 'px';
		this.progressFrame.style.left = this.dom.offsetLeft + (this.dom.offsetWidth * 0.5 - progressFrameImg.width * 0.5) + 'px';
		this.progressFrame.width = progressFrameImg.width;
		this.progressFrame.height = progressFrameImg.height;

		this.progressBarImg.style.top = '0px';
		this.progressBarImg.style.left = '0px';
		this.progressBarImg.width = progressFrameImg.width;
		this.progressBarImg.height = progressFrameImg.height;

		this.progressBar.style.top = this.progressFrame.style.top;
		this.progressBar.style.left = this.progressFrame.style.left;
		this.progressBar.style.width = (progressFrameImg.width * this.progress) + 'px';
		this.progressBar.style.height = progressFrameImg.height + 'px';

		this.messageArea.style.top = this.progressFrame.style.top;
		this.messageArea.style.left = 0;
		this.messageArea.style.width = '100%';
		this.messageArea.style.textAlign = 'center';
		this.messageArea.innerHTML = this.message;
	}

	this.Update ();
}