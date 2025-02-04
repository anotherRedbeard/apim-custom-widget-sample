import React, {useEffect, useState} from "react"
import {useRequest, useExternalApiRequest, useSecrets, useValues} from "../hooks"

const App = () => {
  const values = useValues()
  const {userId, token, apiVersion, managementApiUrl} = useSecrets()
  const [apis, setApis] = useState<any[]>([])
  const request = useRequest()
  const externalApiRequest = useExternalApiRequest()

  const [defaultUserName, setDefaultUserName] = useState<string | undefined>()
  const [displayName, setDisplayName] = useState<string>("");
  const [replyUrls, setReplyUrls] = useState<string>("");
  const [description, setDescription] = useState<string>("");

  useEffect(() => {
    if (!userId) {
      setDefaultUserName("")
      return
    }

    console.log("Fetching user data...", token)
    console.log("userId...", userId)
    console.log("apiVersion...", apiVersion)
    console.log("managementapiurl...", managementApiUrl)

    //get request to fetch all app registrations
    externalApiRequest(`/GetAppRegistration/`)
      .then(e => e.json())
      .then(e => {
        console.log("Apis fetched!", e.Value)
        setApis(e.Value)
      })
      .catch(e => {
        console.error("Could not fetch apis!", e)
        setApis([])
      }
    )

    // example of a get request to fetch user data from the APIM instance, this data isn't used in this example
    request(`/users/${userId}`)
      .then(e => e.json())
      .then(res => {
        console.log("Fetched user data!", res.properties)
        setDefaultUserName(res.properties.firstName)
      })
      .catch(e => {
        console.error("Could not prefill the email address!", e)
        setDefaultUserName("")
      })
  }, [userId, request])

  const handlePostAppRegistration = (event: React.FormEvent) => {
    event.preventDefault();

    // request body for the post request, notice we are only using the displayName, description and replyUrls
    const requestBody = {
      displayName: displayName,
      description: description, 
      spa: {
        redirectUris: replyUrls.split(",").map(url => url.trim())
      }
    }

    // post request to create a new app registration based on the request body
    externalApiRequest(`/PostAppRegistration/`, {
      method: 'POST',
      body: requestBody
    })
      .then(response => response.json())
      .then(data => {
        console.log("App registration posted!", data);
      })
      .catch(error => {
        console.error("Error posting app registration!", error);
      })
  }

  if (defaultUserName == undefined) return <div><p>For some reason we were not able to log you in. Please try logging in again.</p></div>

  return (
    <div>
      <h1>API Management</h1>
      <p>Hi <span>{defaultUserName || "there"}</span>, complete the following section to create a new app registration:</p>
      <form onSubmit={handlePostAppRegistration}>
        <div className="form-group">
          <label htmlFor="displayName" className="form-label">Display Name</label>
          <input
            id="displayName"
            type="text"
            name="displayName"
            placeholder="App Display Name"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            className="form-control"
          />
        </div>
        <div className="form-group">
          <label htmlFor="description" className="form-label">Description</label>
          <textarea
            id="description"
            name="description"
            placeholder="Description of you application"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="form-control flex-grow"
          />
        </div>
        <div className="form-group">
          <label htmlFor="replyUrls" className="form-label">Reply Url(s)</label>
          <textarea
            id="replyUrls"
            name="replyUrls"
            placeholder="comma separated reply urls"
            value={replyUrls}
            onChange={(e) => setReplyUrls(e.target.value)}
            className="form-control flex-grow"
          />
        </div>
        <button type="submit" className="button button-primary">Post App Registration</button>
      </form>

      <h2>App Registrations</h2>
      <ul>
        {apis.map((api) => (
          <li key={api.AppId}>
            <h3>{api.DisplayName}</h3>
          </li>
        ))}
      </ul>
    </div>
  )
}

export default App
