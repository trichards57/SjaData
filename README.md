<a id="readme-top"></a>

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

<br />
<div align="center">
<h3 align="center">SJA in Numbers</h3>

  <p align="center">
    Management Information Dashboard for SJA Volunteers
    <br />
    <a href="https://github.com/trichards57/SjaData/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    ·
    <a href="https://github.com/trichards57/SjaData/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#build">Build</a></li>
        <li><a href="#deployment">Deployment</a></li>
      </ul>
    </li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

Managment Information Dashboard for SJA volunteering, aiming to provide a simple and easy to use interface for volunteer managerss to access data and statistics.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

### Prerequisites

This tool has been built using .NET 8.0.  You will need to have the .NET SDK installed on your machine to build and run the project.
The hosting server will need the ASP .NET Core Runtime and the .NET Core Hosting Bundle installed.

https://dotnet.microsoft.com/en-us/download/dotnet/8.0

### Build

  ```powershell
  dotnet build
  ```

### Test

  ```powershell
  dotnet test
  ```

### Deployment

This tool can be deployed using Web Deploy or by copying the files to the server.

The following configuration values will be required:

* ```ConnectionStrings:DefaultConnection``` - The connection string to the database.  This should be an SQL Server connection string.
* ```Authentication:Microsoft:ClientSecret``` - The secret key for the Azure Entra application.
* ```Authentication:Microsoft:ClientId``` - The client ID for the Azure Entra application.
* ```Authentication:Microsoft:TenantId``` - The tenant ID for the Azure Entra application.
* ```APPLICATIONINSIGHTS_CONNECTION_STRING``` - The connection string for the Azure Application Insights instance.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ROADMAP -->
## Roadmap

- [ ] Hours Statistics
- [ ] People Reports
- [ ] Vehicle VOR Information
- [ ] Hub Mananagement Information

See the [open issues](https://github.com/trichards57/SjaData/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Top contributors:

<a href="https://github.com/trichards57/SjaData/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=trichards57/SjaData" alt="contrib.rocks image" />
</a>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Tony Richards - [@trichards57](https://twitter.com/trichards57) - trichards57@pm.me

Project Link: [https://github.com/trichards57/SjaData](https://github.com/trichards57/SjaData)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/trichards57/SjaData.svg?style=for-the-badge
[contributors-url]: https://github.com/trichards57/SjaData/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/trichards57/SjaData.svg?style=for-the-badge
[forks-url]: https://github.com/trichards57/SjaData/network/members
[stars-shield]: https://img.shields.io/github/stars/trichards57/SjaData.svg?style=for-the-badge
[stars-url]: https://github.com/trichards57/SjaData/stargazers
[issues-shield]: https://img.shields.io/github/issues/trichards57/SjaData.svg?style=for-the-badge
[issues-url]: https://github.com/trichards57/SjaData/issues
[license-shield]: https://img.shields.io/github/license/trichards57/SjaData.svg?style=for-the-badge
[license-url]: https://github.com/trichards57/SjaData/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/anthony-richards-394ba866
