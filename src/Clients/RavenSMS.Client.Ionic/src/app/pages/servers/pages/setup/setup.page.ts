import { BarcodeScanner, SupportedFormat } from '@capacitor-community/barcode-scanner';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { IRegisterServerInfoModel } from 'src/app/core/models';
import { ServersStoreActions } from 'src/app/store/';
import { AlertController } from '@ionic/angular';
import { RootStoreState } from 'src/app/store';
import { Capacitor } from '@capacitor/core';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { SubSink } from 'subsink';

@Component({
  selector: 'page-server-setup',
  styleUrls: ['./setup.page.scss'],
  templateUrl: 'setup.page.html'
})
export class SetupPage {

  subsink = new SubSink();

  DEBUG_MESSAGE: string;
  messageTranslationKey: string;
  scanActive = false;
  permissionGranted = false;
  configurationAllowed = false;
  showOpenSettingsButton = false;
  showGrantPermissionButton = false;

  platform: string;
  configurationForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private alert: AlertController,
    private store: Store<RootStoreState.State>,
  ) {
    // get the platform
    this.platform = Capacitor.getPlatform();
  }

  async ionViewDidEnter(): Promise<void> {
    // this code is only relevant if we are not previewing in a web browser
    if (this.platform != 'web') {
      // first check for the user permission
      this.messageTranslationKey = 'servers.pages.setup.messages.checking_permission';

      this.permissionGranted = await this.checkUserPermission();
      if (this.permissionGranted) {
        this.showGrantPermissionButton = false;
        this.messageTranslationKey = 'servers.pages.setup.messages.ready_for_scan';

        // prepare the barcode scanner
        BarcodeScanner.prepare();
      }

      return;
    }

    this.initializeForm();
  }

  ionViewDidLeave(): void {
    this.subsink.unsubscribe();

    if (this.platform != 'web') {
      BarcodeScanner.stopScan();
    }
  }

  async startScanning(): Promise<void> {
    this.scanActive = true;
    this.messageTranslationKey = 'servers.pages.setup.messages.start_scanning';

    // start the scanning
    const scanResult = await BarcodeScanner.startScan({
      targetedFormats: [
        SupportedFormat.QR_CODE
      ],
    });

    // set the scanning state to false
    this.scanActive = false;

    // if the result has content
    if (scanResult.hasContent) {
      // read the json content from the QR content
      const model = JSON.parse(atob(scanResult.content)) as IRegisterServerInfoModel;

      // check if we have any valid content
      if (!model.serverUrl || !model.clientId) {
        this.messageTranslationKey = 'servers.pages.setup.messages.invalid_qr_value';
        return;
      }

      // dispatch the add server action
      this.store.dispatch(ServersStoreActions.AddServer({ serverInfo: model }));

      this.messageTranslationKey = 'servers.pages.setup.messages.done';
      return;
    }

    this.messageTranslationKey = 'servers.pages.setup.messages.failed_to_read_qr_value';
  }

  async checkUserPermission(force: boolean = false): Promise<boolean> {
    // check if user already granted permission
    const status = await BarcodeScanner.checkPermission({ force });

    // user granted permission
    if (status.granted) {
      return true;
    }

    // user denied permission
    // (restricted & unknown) probably means the permission has been denied (ios only)
    if (status.denied || status.restricted || status.unknown) {
      this.messageTranslationKey = 'servers.pages.setup.messages.permission_denied';
      this.showOpenSettingsButton = true;
      return false;
    }

    // user has not been requested this permission before
    if (status.neverAsked) {
      const alertResult = await this.alert.create({
        header: "Permission request",
        message: "allow camera permission to enable the QR code scanner",
        buttons: [
          {
            text: "NO",
            role: 'cancel',
          },
          {
            text: "OK",
            role: 'accept',
          }
        ]
      });

      // show the alert
      await alertResult.present();

      // get the result on dismiss
      const dismissResult = await alertResult.onDidDismiss();
      if (dismissResult.role == "cancel") {
        this.messageTranslationKey = "servers.pages.setup.messages.permission_request_canceled";
        this.showGrantPermissionButton = true;
        return false;
      }
    }

    // user did not grant the permission, so he must have declined the request
    return await this.checkUserPermission(true);
  }

  async grantPermissionAsync() {
    this.permissionGranted = await this.checkUserPermission();
    if (this.permissionGranted) {
      this.showGrantPermissionButton = false;
    }
  }

  openSettingPage() {
    BarcodeScanner.openAppSettings();
  }

  initializeForm(): void {
    this.configurationForm = this.fb.group({
      clientId: this.fb.control('', [
        Validators.required,
        Validators.maxLength(17),
        Validators.minLength(17),
      ]),
      serverId: this.fb.control('', [
        Validators.required,
      ]),
      serverUrl: this.fb.control('', [
        Validators.required,
        Validators.pattern(/^https?:\/\/\w+(\.\w+)*(:[0-9]+)?(\/.*)?$/)
      ]),
    });
  }

  submit(): void {
    if (!this.configurationForm.valid) {
      this.messageTranslationKey = 'servers.pages.setup.messages.invalid_setup_form_value';
      return;
    }

    var model = this.configurationForm.value as IRegisterServerInfoModel;

    // check if we have any valid content
    if (!model.serverUrl || !model.clientId) {
      this.messageTranslationKey = 'servers.pages.setup.messages.invalid_setup_form_value';
      return;
    }

    // dispatch the configuration action
    this.store.dispatch(ServersStoreActions.AddServer({ serverInfo: model }));
  }
}
