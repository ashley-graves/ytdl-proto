// ==UserScript==
// @name            Youtube Downloader
// @description     Adds a link on youtube pages to download the video via ytdl-proto.
//
// @author          Yuki
// @namespace       https://github.com/catgirlyuki
//
// @version         1.0
//
// @match           http*://*.youtube.com/*
// @grant           none
// ==/UserScript==

var icondl = '<svg viewBox="0 0 24 24" preserveAspectRatio="xMidYMid meet" focusable="false" class="style-scope yt-icon" style="pointer-events: none; display: block; width: 100%; height: 100%;"><g mirror-in-rtl="" class="style-scope yt-icon"><rect fill="none" height="24" width="24"/></g><g><path d="M5,20h14v-2H5V20z M19,9h-4V3H9v6H5l7,7L19,9z"/></g></svg>';

function createElementFromHTML(htmlString) {
  var div = document.createElement('div');
  div.innerHTML = htmlString.trim();

  // Change this to div.childNodes to support multiple top-level nodes
  return div.firstChild;
}

setInterval(function() {
    var parent = document.querySelector("ytd-video-primary-info-renderer ytd-menu-renderer > div");
    if(parent == null) return;

    if(document.querySelector("#download-btn") != null) {
        let elem = document.querySelectorAll("#download-btn");
        let doReturn = false;
        for(let i = 0; i < elem.length; i++) {
            if(elem[i].parentElement != parent) {
                elem[i].remove();
            } else {
                doReturn = true;
            }
        }
        if(doReturn) return;
    }

    var item = document.createElement("a");
    item.classList = "yt-simple-endpoint style-scope ytd-button-renderer";
    item.href = "ytdl://"+location.href;

    var iconItem = document.createElement("yt-icon-button");
    iconItem.id = "button";
    iconItem.classList = "style-scope ytd-button-renderer style-default size-default";

    var iconsvg = document.createElement("yt-icon");
    iconItem.appendChild(iconsvg);
    item.appendChild(iconItem);

    var labelItem = document.createElement("yt-formatted-string");
    labelItem.classList = "style-scope ytd-button-renderer style-default size-default";
    labelItem.id = "text";
    labelItem.innerHTML = "Download";
    item.appendChild(labelItem);

    var tooltipItem = document.createElement("tp-yt-paper-tooltip");
    tooltipItem.classList = "style-scope ytd-button-renderer";

    var tooltipText = document.createElement("div");
    tooltipText.classList = "style-scope tp-yt-paper-tooltip hidden";
    tooltipText.id = "tooltip";
    tooltipText.classList = "style-scope ytd-button-renderer";
    tooltipText.innerText = "Download";
    tooltipItem.appendChild(tooltipText);

    item.appendChild(tooltipItem);

    var renderer = document.createElement("ytd-button-renderer");
    renderer.classList = "style-scope ytd-menu-renderer force-icon-button style-default size-default";
    renderer.setAttribute("is-icon-button", "");
    renderer.setAttribute("style-action-button", "");
    renderer.setAttribute("button-renderer", "true");
    renderer.setAttribute("tabindex", "1");
    renderer.setAttribute("aria-selected", "false");

    renderer.appendChild(item);

    renderer.id = "download-btn";

    parent.appendChild(renderer);
    renderer.appendChild(item);
    item.appendChild(labelItem);
    labelItem.innerHTML = "Download";
    iconsvg.innerHTML = icondl;
}, 1);
