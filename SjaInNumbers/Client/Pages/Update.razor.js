let dotNetHelper;

export function setup(helper) {
    const dropSite = document.getElementById("drop-site");
    dropSite.addEventListener("drop", fileDrop);
    dotNetHelper = helper;
}

function fileDrop(args) {
    if (args.dataTransfer.files.length !== 1) {
        return;
    }

    handleFile(args.dataTransfer.files[0]);
}

async function uploadHours(file) {
    const formData = new FormData();
    formData.append("file", file);

    const res = await fetch("/api/hours", {
        method: "POST",
        body: formData,
    });

    return res.ok ? (await res.json()) : undefined;
};

async function uploadPeople(file) {
    const formData = new FormData();
    formData.append("file", file);

    const res = await fetch("/api/people", {
        method: "POST",
        body: formData,
    });

    return res.ok ? (await res.json()) : undefined;
};

async function uploadDeployments(file) {
    const formData = new FormData();
    formData.append("file", file);

    const res = await fetch("/api/deployments", {
        method: "POST",
        body: formData,
    });

    return res.ok ? (await res.json()) : undefined;
};

async function handleFile(file) {
    const firstBytes = await file.slice(0, 6, "text/plain").text();

    if (firstBytes.toUpperCase() === "MYDATA") {
        dotNetHelper.invokeMethodAsync("ShowUploading");
        try {
            const result = await uploadPeople(file);

            if (result) {
                await dotNetHelper.invokeMethodAsync("ShowUploadSuccess", result.count, "people");
            } else {
                await dotNetHelper.invokeMethodAsync("ShowUploadFailed");
            }
        } catch {
            await dotNetHelper.invokeMethodAsync("ShowUploadFailed");
        }
    } else if (firstBytes.toUpperCase() === "LOCATI") {
        await dotNetHelper.invokeMethodAsync("ShowUploading");
        try {
            const result = await uploadHours(file);

            if (result) {
                await dotNetHelper.invokeMethodAsync("ShowUploadSuccess", result.count, "hours");
            } else {
                await dotNetHelper.invokeMethodAsync("ShowUploadFailed");
            }
        } catch {
            await dotNetHelper.invokeMethodAsync("ShowUploadFailed");
        }
    } else if (firstBytes.toUpperCase() === "AOB") {
        await dotNetHelper.invokeMethodAsync("ShowUploading");
        try {
            const result = await uploadDeployments(file);

            if (result) {
                await dotNetHelper.invokeMethodAsync("ShowUploadSuccess", result.count, "deployments");
            } else {
                await dotNetHelper.invokeMethodAsync("ShowUploadFailed");
            }
        } catch {
            await dotNetHelper.invokeMethodAsync("ShowUploadFailed");
        }
    } else {
        await dotNetHelper.invokeMethodAsync("ShowInvalidFile");
    }
}
